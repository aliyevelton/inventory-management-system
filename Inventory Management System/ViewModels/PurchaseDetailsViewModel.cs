namespace Inventory_Management_System.ViewModels;

public class PurchaseDetailsViewModel
{
    public int Id { get; set; }
    public string SupplierName { get; set; } = null!;
    public DateTime PurchaseDate { get; set; }
    public string Reference { get; set; } = null!;
    public decimal TotalCost { get; set; }
    public string? AppUserName { get; set; }

    public List<PurchaseItemDetails> PurchaseItems { get; set; } = new List<PurchaseItemDetails>();

    public class PurchaseItemDetails
    {
        public string ProductName { get; set; } = null!;
        public string ProductImageFileName { get; set; } = "product-default.png";
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
    }
}