namespace MyWinFormsApp.Models;

public class CreditNoteItem
{
    public int Id { get; set; }
    public int CreditNoteId { get; set; }
    public int? InvoiceItemId { get; set; }
    public int? ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineTotal { get; set; }
    public DateTime CreatedAt { get; set; }

    // For return form UI
    public decimal MaxQuantity { get; set; }
    public decimal ReturnQuantity { get; set; }
    public bool IsSelected { get; set; }
}
