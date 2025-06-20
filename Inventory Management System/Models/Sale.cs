using Inventory_Management_System.Models;
using Inventory_Management_System.Enums;

public class Sale
{
    public int Id { get; set; }

    public string AppUserId { get; set; } = null!; //this is for who created the sale, not who bought it
    public AppUser AppUser { get; set; } = null!;
    public decimal TotalPrice { get; set; }
    public decimal? DiscountAmount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string Reference { get; set; } = null!; 
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
}