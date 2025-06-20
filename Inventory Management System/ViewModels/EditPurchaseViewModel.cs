namespace Inventory_Management_System.ViewModels;

public class EditPurchaseViewModel
{
    public int Id { get; set; }
    public string SupplierName { get; set; } = null!;
    public DateTime PurchaseDate { get; set; }
    public string Reference { get; set; } = null!;
    public string? AppUserId { get; set; } // For selecting who made the purchase

    public List<PurchaseItemInput> PurchaseItems { get; set; } = new List<PurchaseItemInput>();

    public class PurchaseItemInput
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; } 
        public string ProductImageFileName { get; set; } = "product-default.png";
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
    }
}
