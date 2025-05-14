namespace Inventory_Management_System.Models;

public class Sale
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public string AppUserId { get; set; } = null!;
    public AppUser AppUser { get; set; } = null!;
    public int QuantitySold { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime CreatedDate { get; set; }
}

