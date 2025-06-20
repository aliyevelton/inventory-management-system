namespace Inventory_Management_System.ViewModels;

public class CreatePurchaseViewModel
{
    public string SupplierName { get; set; } = null!;
    public DateTime PurchaseDate { get; set; } = DateTime.Now;
    public string Reference { get; set; } = null!;
    public List<PurchaseItemInput> PurchaseItems { get; set; } = new List<PurchaseItemInput>
    {
        new PurchaseItemInput()
    };

    public class PurchaseItemInput
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
    }
}
