using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.ViewModels;

public class ProductUpdateViewModel
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    public string? Description { get; set; }

    [Required]
    public decimal Price { get; set; }

    public decimal? DiscountedPrice { get; set; }

    public string? SKU { get; set; }


    [Required]
    public int CategoryId { get; set; }

    [Required]
    public int BrandId { get; set; }

    public bool IsActive { get; set; }

    public List<IFormFile>? ProductImages { get; set; }
    public List<string>? ExistingImageNames { get; set; }
    public List<int>? ExistingImageIds { get; set; }
}

