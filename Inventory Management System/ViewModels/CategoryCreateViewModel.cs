using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.ViewModels;

public class CategoryCreateViewModel
{
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public string Description { get; set; }

    [Required]
    public string Code { get; set; } = null!;
}
