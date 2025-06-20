namespace Inventory_Management_System.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    public string SKU { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string BrandName { get; set; } = null!;
    public string CategoryName { get; set; } = null!;
    public string Image { get; set; } = null!;
    public decimal Price { get; set; }
}
