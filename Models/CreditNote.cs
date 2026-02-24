namespace MyWinFormsApp.Models;

public class CreditNote
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public string CreditNoteNumber { get; set; } = string.Empty;
    public int InvoiceId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int? CustomerId { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = "ISSUED";
    public int? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }

    // Joined
    public string InvoiceNumber { get; set; } = string.Empty;
    public string CreatedByName { get; set; } = string.Empty;
    public int ItemCount { get; set; }

    // Computed
    public string FormattedTotal => $"₹{TotalAmount:N2}";
    public string FormattedDate => CreatedAt.ToString("dd MMM yyyy hh:mm tt");
    public string StatusDisplay => Status switch
    {
        "ISSUED" => "Issued",
        "VOID" => "Void",
        _ => Status
    };
}
