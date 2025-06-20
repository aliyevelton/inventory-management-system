namespace Inventory_Management_System.Models;

public class ProductImage
{
    public int Id { get; set; }
    public string Image { get; set; } = null!;
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
}
