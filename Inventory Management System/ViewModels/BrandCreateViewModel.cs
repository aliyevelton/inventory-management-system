using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.ViewModels;

public class BrandCreateViewModel
{
    [Required]
    public string Name { get; set; }

    public string? Description { get; set; }

    [Required]
    public IFormFile LogoFile { get; set; }
}
