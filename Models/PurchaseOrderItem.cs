namespace MyWinFormsApp.Models;

public class PurchaseOrderItem
{
    public int Id { get; set; }
    public int PurchaseOrderId { get; set; }
    public int? ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineTotal { get; set; }
    public decimal ReceivedQuantity { get; set; }
    public DateTime CreatedAt { get; set; }

    // UI helpers
    public decimal PendingQuantity => Quantity - ReceivedQuantity;
    public string FormattedLineTotal => $"₹{LineTotal:N2}";
}
