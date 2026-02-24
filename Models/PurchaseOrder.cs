namespace MyWinFormsApp.Models;

public class PurchaseOrder
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public string PoNumber { get; set; } = string.Empty;
    public int? SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "DRAFT";
    public string Notes { get; set; } = string.Empty;
    public DateTime? ExpectedDate { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Joined
    public string CreatedByName { get; set; } = string.Empty;
    public int ItemCount { get; set; }

    // Computed
    public string FormattedTotal => $"₹{TotalAmount:N2}";
    public string FormattedDate => CreatedAt.ToString("dd MMM yyyy");
    public string StatusDisplay => Status switch
    {
        "DRAFT" => "Draft",
        "ORDERED" => "Ordered",
        "PARTIAL" => "Partially Received",
        "RECEIVED" => "Received",
        "CANCELLED" => "Cancelled",
        _ => Status
    };
    public string ExpectedDateDisplay => ExpectedDate?.ToString("dd MMM yyyy") ?? "-";
}
