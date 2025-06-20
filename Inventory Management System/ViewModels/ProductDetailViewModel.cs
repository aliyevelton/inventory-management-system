namespace Inventory_Management_System.ViewModels;

public class ProductDetailViewModel
{
    public string Name { get; set; }
    public string CategoryName { get; set; }
    public string BrandName { get; set; }
    public string SKU { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountedPrice { get; set; }
    public bool IsActive { get; set; }
    public string Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public List<ProductImageViewModel> Images { get; set; }
}