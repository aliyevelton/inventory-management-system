namespace Inventory_Management_System.Models;

public class Brand
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public bool IsDeleted { get; set; }
    public string? Logo { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public ICollection<Product>? Products { get; set; }
}
