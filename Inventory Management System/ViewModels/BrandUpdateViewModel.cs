namespace Inventory_Management_System.ViewModels;

public class BrandUpdateViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? ExistingLogo { get; set; }
    public IFormFile? LogoFile { get; set; }
}
