using Dapper;
using MyWinFormsApp.Helpers;
using MyWinFormsApp.Models;

namespace MyWinFormsApp.Services;

public static class ReturnService
{
    public static async Task<string> GetNextCreditNoteNumberAsync(int tenantId)
    {
        var nextSeq = await DatabaseHelper.ExecuteScalarAsync<int>(@"
            INSERT INTO credit_note_sequences (tenant_id, last_sequence)
            VALUES (@TenantId, 1)
            ON CONFLICT (tenant_id)
            DO UPDATE SET last_sequence = credit_note_sequences.last_sequence + 1, updated_at = NOW()
            RETURNING last_sequence", new { TenantId = tenantId });

        return $"CN-{nextSeq:D6}";
    }

    public static async Task<(bool Success, string Message, CreditNote? CreditNote)> CreateReturnAsync(
        int tenantId, int invoiceId, string customerName, int? customerId,
        string reason, List<CreditNoteItem> items, int? userId)
    {
        using var connection = DatabaseHelper.GetConnection();
        await connection.OpenAsync();
        using var transaction = await connection.BeginTransactionAsync();

        try
        {
            var creditNoteNumber = await GetNextCreditNoteNumberAsync(tenantId);

            decimal subtotal = items.Sum(i => i.Quantity * i.UnitPrice);
            decimal taxAmount = items.Sum(i => i.TaxAmount);
            decimal totalAmount = subtotal + taxAmount;

            var creditNote = new CreditNote
            {
                TenantId = tenantId,
                CreditNoteNumber = creditNoteNumber,
                InvoiceId = invoiceId,
                CustomerName = customerName,
                CustomerId = customerId,
                Subtotal = subtotal,
                TaxAmount = taxAmount,
                TotalAmount = totalAmount,
                Reason = reason,
                Status = "ISSUED",
                CreatedBy = userId
            };

            creditNote.Id = await connection.ExecuteScalarAsync<int>(@"
                INSERT INTO credit_notes (
                    tenant_id, credit_note_number, invoice_id, customer_name, customer_id,
                    subtotal, tax_amount, total_amount, reason, status, created_by
                ) VALUES (
                    @TenantId, @CreditNoteNumber, @InvoiceId, @CustomerName, @CustomerId,
                    @Subtotal, @TaxAmount, @TotalAmount, @Reason, @Status, @CreatedBy
                ) RETURNING id", creditNote, transaction);

            foreach (var item in items)
            {
                item.CreditNoteId = creditNote.Id;
                item.LineTotal = item.Quantity * item.UnitPrice + item.TaxAmount;

                await connection.ExecuteAsync(@"
                    INSERT INTO credit_note_items (
                        credit_note_id, invoice_item_id, product_id, product_name,
                        quantity, unit_price, tax_rate, tax_amount, line_total
                    ) VALUES (
                        @CreditNoteId, @InvoiceItemId, @ProductId, @ProductName,
                        @Quantity, @UnitPrice, @TaxRate, @TaxAmount, @LineTotal
                    )", item, transaction);

                // Restore stock
                if (item.ProductId.HasValue)
                {
                    var product = await connection.QueryFirstOrDefaultAsync<Product>(
                        "SELECT current_stock FROM products WHERE id = @Id",
                        new { Id = item.ProductId }, transaction);

                    var prevStock = product?.CurrentStock ?? 0;

                    await connection.ExecuteAsync(
                        "UPDATE products SET current_stock = current_stock + @Qty, updated_at = NOW() WHERE id = @Id",
                        new { Qty = item.Quantity, Id = item.ProductId }, transaction);

                    await connection.ExecuteAsync(@"
                        INSERT INTO stock_movements (tenant_id, product_id, movement_type, quantity, previous_stock, new_stock, reference, notes, created_by)
                        VALUES (@TenantId, @ProductId, 'IN', @Quantity, @PrevStock, @NewStock, @Reference, @Notes, @CreatedBy)",
                        new
                        {
                            TenantId = tenantId,
                            item.ProductId,
                            item.Quantity,
                            PrevStock = prevStock,
                            NewStock = prevStock + item.Quantity,
                            Reference = $"Return {creditNoteNumber}",
                            Notes = $"Return - {item.ProductName}",
                            CreatedBy = userId
                        }, transaction);
                }
            }

            await transaction.CommitAsync();
            creditNote.CreatedAt = DateTime.Now;
            return (true, "Return processed successfully!", creditNote);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return (false, $"Failed to process return: {ex.Message}", null);
        }
    }

    public static async Task<List<CreditNote>> GetCreditNotesAsync(int tenantId, int limit = 100)
    {
        var results = await DatabaseHelper.QueryAsync<CreditNote>(@"
            SELECT cn.*, i.invoice_number, u.full_name as created_by_name,
                   (SELECT COUNT(*)::int FROM credit_note_items WHERE credit_note_id = cn.id) as item_count
            FROM credit_notes cn
            LEFT JOIN invoices i ON cn.invoice_id = i.id
            LEFT JOIN users u ON cn.created_by = u.id
            WHERE cn.tenant_id = @TenantId
            ORDER BY cn.created_at DESC
            LIMIT @Limit",
            new { TenantId = tenantId, Limit = limit });
        return results.ToList();
    }

    public static async Task<List<CreditNoteItem>> GetCreditNoteItemsAsync(int creditNoteId)
    {
        var results = await DatabaseHelper.QueryAsync<CreditNoteItem>(
            "SELECT * FROM credit_note_items WHERE credit_note_id = @Id ORDER BY id",
            new { Id = creditNoteId });
        return results.ToList();
    }
}
