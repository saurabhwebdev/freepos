using Dapper;
using MyWinFormsApp.Helpers;
using MyWinFormsApp.Models;

namespace MyWinFormsApp.Services;

public static class PurchaseOrderService
{
    public static async Task<string> GetNextPoNumberAsync(int tenantId)
    {
        var nextSeq = await DatabaseHelper.ExecuteScalarAsync<int>(@"
            INSERT INTO po_sequences (tenant_id, last_sequence)
            VALUES (@TenantId, 1)
            ON CONFLICT (tenant_id)
            DO UPDATE SET last_sequence = po_sequences.last_sequence + 1, updated_at = NOW()
            RETURNING last_sequence", new { TenantId = tenantId });

        return $"PO-{nextSeq:D6}";
    }

    public static async Task<(bool Success, string Message, PurchaseOrder? Po)> CreateAsync(
        PurchaseOrder po, List<PurchaseOrderItem> items)
    {
        using var connection = DatabaseHelper.GetConnection();
        await connection.OpenAsync();
        using var transaction = await connection.BeginTransactionAsync();

        try
        {
            po.PoNumber = await GetNextPoNumberAsync(po.TenantId);
            po.Subtotal = items.Sum(i => i.Quantity * i.UnitPrice);
            po.TaxAmount = items.Sum(i => i.TaxAmount);
            po.TotalAmount = po.Subtotal + po.TaxAmount;

            po.Id = await connection.ExecuteScalarAsync<int>(@"
                INSERT INTO purchase_orders (
                    tenant_id, po_number, supplier_id, supplier_name,
                    subtotal, tax_amount, total_amount, status, notes, expected_date, created_by
                ) VALUES (
                    @TenantId, @PoNumber, @SupplierId, @SupplierName,
                    @Subtotal, @TaxAmount, @TotalAmount, @Status, @Notes, @ExpectedDate, @CreatedBy
                ) RETURNING id", po, transaction);

            foreach (var item in items)
            {
                item.PurchaseOrderId = po.Id;
                item.LineTotal = item.Quantity * item.UnitPrice + item.TaxAmount;

                await connection.ExecuteAsync(@"
                    INSERT INTO purchase_order_items (
                        purchase_order_id, product_id, product_name,
                        quantity, unit_price, tax_rate, tax_amount, line_total
                    ) VALUES (
                        @PurchaseOrderId, @ProductId, @ProductName,
                        @Quantity, @UnitPrice, @TaxRate, @TaxAmount, @LineTotal
                    )", item, transaction);
            }

            await transaction.CommitAsync();
            po.CreatedAt = DateTime.Now;
            return (true, "Purchase order created!", po);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return (false, $"Failed to create PO: {ex.Message}", null);
        }
    }

    public static async Task<List<PurchaseOrder>> GetAllAsync(int tenantId, int limit = 100)
    {
        var results = await DatabaseHelper.QueryAsync<PurchaseOrder>(@"
            SELECT po.*, u.full_name as created_by_name,
                   (SELECT COUNT(*)::int FROM purchase_order_items WHERE purchase_order_id = po.id) as item_count
            FROM purchase_orders po
            LEFT JOIN users u ON po.created_by = u.id
            WHERE po.tenant_id = @TenantId
            ORDER BY po.created_at DESC
            LIMIT @Limit",
            new { TenantId = tenantId, Limit = limit });
        return results.ToList();
    }

    public static async Task<(PurchaseOrder? Po, List<PurchaseOrderItem> Items)> GetWithItemsAsync(int poId)
    {
        var po = await DatabaseHelper.QueryFirstOrDefaultAsync<PurchaseOrder>(
            "SELECT * FROM purchase_orders WHERE id = @Id", new { Id = poId });

        if (po == null)
            return (null, new List<PurchaseOrderItem>());

        var items = await DatabaseHelper.QueryAsync<PurchaseOrderItem>(
            "SELECT * FROM purchase_order_items WHERE purchase_order_id = @PoId ORDER BY id",
            new { PoId = poId });

        return (po, items.ToList());
    }

    public static async Task<(bool Success, string Message)> ReceiveItemsAsync(
        int tenantId, int poId, List<(int ItemId, decimal ReceivedQty)> receivals, int? userId)
    {
        using var connection = DatabaseHelper.GetConnection();
        await connection.OpenAsync();
        using var transaction = await connection.BeginTransactionAsync();

        try
        {
            var po = await connection.QueryFirstOrDefaultAsync<PurchaseOrder>(
                "SELECT * FROM purchase_orders WHERE id = @Id", new { Id = poId }, transaction);
            if (po == null) return (false, "Purchase order not found.");

            foreach (var (itemId, receivedQty) in receivals)
            {
                if (receivedQty <= 0) continue;

                var item = await connection.QueryFirstOrDefaultAsync<PurchaseOrderItem>(
                    "SELECT * FROM purchase_order_items WHERE id = @Id",
                    new { Id = itemId }, transaction);
                if (item == null) continue;

                var newReceived = item.ReceivedQuantity + receivedQty;
                await connection.ExecuteAsync(
                    "UPDATE purchase_order_items SET received_quantity = @Received WHERE id = @Id",
                    new { Received = newReceived, Id = itemId }, transaction);

                // Update product stock
                if (item.ProductId.HasValue)
                {
                    var product = await connection.QueryFirstOrDefaultAsync<Product>(
                        "SELECT current_stock FROM products WHERE id = @Id",
                        new { Id = item.ProductId }, transaction);

                    var prevStock = product?.CurrentStock ?? 0;

                    await connection.ExecuteAsync(
                        "UPDATE products SET current_stock = current_stock + @Qty, updated_at = NOW() WHERE id = @Id",
                        new { Qty = receivedQty, Id = item.ProductId }, transaction);

                    await connection.ExecuteAsync(@"
                        INSERT INTO stock_movements (tenant_id, product_id, movement_type, quantity, previous_stock, new_stock, reference, notes, created_by)
                        VALUES (@TenantId, @ProductId, 'IN', @Quantity, @PrevStock, @NewStock, @Reference, @Notes, @CreatedBy)",
                        new
                        {
                            TenantId = tenantId,
                            item.ProductId,
                            Quantity = receivedQty,
                            PrevStock = prevStock,
                            NewStock = prevStock + receivedQty,
                            Reference = $"PO {po.PoNumber}",
                            Notes = $"PO Receive - {item.ProductName}",
                            CreatedBy = userId
                        }, transaction);
                }
            }

            // Update PO status
            var allItems = await connection.QueryAsync<PurchaseOrderItem>(
                "SELECT * FROM purchase_order_items WHERE purchase_order_id = @PoId",
                new { PoId = poId }, transaction);

            var itemList = allItems.ToList();
            bool allReceived = itemList.All(i => i.ReceivedQuantity >= i.Quantity);
            bool anyReceived = itemList.Any(i => i.ReceivedQuantity > 0);

            var newStatus = allReceived ? "RECEIVED" : anyReceived ? "PARTIAL" : po.Status;
            await connection.ExecuteAsync(
                "UPDATE purchase_orders SET status = @Status, updated_at = NOW() WHERE id = @Id",
                new { Status = newStatus, Id = poId }, transaction);

            await transaction.CommitAsync();
            return (true, allReceived ? "All items received!" : "Items received successfully.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return (false, $"Failed to receive items: {ex.Message}");
        }
    }

    public static async Task<(bool Success, string Message)> UpdateStatusAsync(int poId, string status)
    {
        try
        {
            await DatabaseHelper.ExecuteAsync(
                "UPDATE purchase_orders SET status = @Status, updated_at = NOW() WHERE id = @Id",
                new { Status = status, Id = poId });
            return (true, "Status updated.");
        }
        catch (Exception ex)
        {
            return (false, $"Failed: {ex.Message}");
        }
    }
}
