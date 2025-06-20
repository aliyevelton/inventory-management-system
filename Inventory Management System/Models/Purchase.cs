namespace Inventory_Management_System.Models;

public class Purchase
{
    public int Id { get; set; }
    public string AppUserId { get; set; } = null!; // User who made the purchase entry
    public AppUser AppUser { get; set; } = null!;
    public string SupplierName { get; set; } = null!;
    public DateTime PurchaseDate { get; set; }
    public string Reference { get; set; } = null!; 
    public decimal TotalCost { get; set; }

    public ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();
}
