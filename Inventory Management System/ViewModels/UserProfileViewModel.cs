using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.ViewModels;

public class UserProfileViewModel
{
    public string Id { get; set; }

    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; }

    [Phone]
    public string? PhoneNumber { get; set; }

    [Required]
    public string UserName { get; set; }

    public string? ExistingImage { get; set; }

    public IFormFile? ProfilePicture { get; set; }
}

