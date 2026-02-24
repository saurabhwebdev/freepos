using MyWinFormsApp.Helpers;
using MyWinFormsApp.Models;

namespace MyWinFormsApp.Services;

public static class CustomerService
{
    public static async Task<List<Customer>> GetCustomersAsync(int tenantId)
    {
        var results = await DatabaseHelper.QueryAsync<Customer>(@"
            SELECT c.*,
                   (SELECT COUNT(*)::int FROM invoices WHERE customer_id = c.id) as invoice_count,
                   (SELECT COALESCE(SUM(total_amount), 0) FROM invoices WHERE customer_id = c.id AND status = 'COMPLETED') as total_spent
            FROM customers c
            WHERE c.tenant_id = @TenantId
            ORDER BY c.name",
            new { TenantId = tenantId });
        return results.ToList();
    }

    public static async Task<List<Customer>> SearchCustomersAsync(int tenantId, string search)
    {
        var results = await DatabaseHelper.QueryAsync<Customer>(@"
            SELECT * FROM customers
            WHERE tenant_id = @TenantId AND is_active = true
              AND (LOWER(name) LIKE @Search OR LOWER(phone) LIKE @Search OR LOWER(email) LIKE @Search)
            ORDER BY name LIMIT 20",
            new { TenantId = tenantId, Search = $"%{search.ToLower()}%" });
        return results.ToList();
    }

    public static async Task<Customer?> GetByIdAsync(int id)
    {
        return await DatabaseHelper.QueryFirstOrDefaultAsync<Customer>(
            "SELECT * FROM customers WHERE id = @Id", new { Id = id });
    }

    public static async Task<(bool Success, string Message)> SaveCustomerAsync(Customer customer)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(customer.Name))
                return (false, "Customer name is required.");

            if (customer.Id == 0)
            {
                customer.Id = await DatabaseHelper.ExecuteScalarAsync<int>(@"
                    INSERT INTO customers (tenant_id, name, phone, email, address, city, state, pin_code, gstin, notes, is_active)
                    VALUES (@TenantId, @Name, @Phone, @Email, @Address, @City, @State, @PinCode, @Gstin, @Notes, @IsActive)
                    RETURNING id", customer);
                return (true, "Customer created.");
            }
            else
            {
                await DatabaseHelper.ExecuteAsync(@"
                    UPDATE customers SET name = @Name, phone = @Phone, email = @Email,
                        address = @Address, city = @City, state = @State, pin_code = @PinCode,
                        gstin = @Gstin, notes = @Notes, is_active = @IsActive, updated_at = NOW()
                    WHERE id = @Id", customer);
                return (true, "Customer updated.");
            }
        }
        catch (Exception ex)
        {
            return (false, $"Error: {ex.Message}");
        }
    }

    public static async Task<(bool Success, string Message)> DeleteCustomerAsync(int id)
    {
        try
        {
            // Check if customer has invoices
            var count = await DatabaseHelper.ExecuteScalarAsync<int>(
                "SELECT COUNT(*)::int FROM invoices WHERE customer_id = @Id", new { Id = id });
            if (count > 0)
                return (false, $"Cannot delete: customer has {count} invoice(s). Deactivate instead.");

            await DatabaseHelper.ExecuteAsync("DELETE FROM customers WHERE id = @Id", new { Id = id });
            return (true, "Customer deleted.");
        }
        catch (Exception ex)
        {
            return (false, $"Error: {ex.Message}");
        }
    }
}
