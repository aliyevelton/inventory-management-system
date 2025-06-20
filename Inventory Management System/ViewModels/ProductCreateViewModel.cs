using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.ViewModels;

public class ProductCreateViewModel
{
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public string Description { get; set; } = null!;

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(0, double.MaxValue)]
    [Display(Name = "Discounted Price")]
    public decimal? DiscountedPrice { get; set; }

    [Required]
    public string SKU { get; set; } = null!;

    [Required]
    public int CategoryId { get; set; }

    [Required]
    public int BrandId { get; set; }


    [Display(Name = "Product Images")]
    public List<IFormFile>? ImageFiles { get; set; }
}
