using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.Areas.Admin.ViewModels;

public class UserCreateViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [StringLength(100, ErrorMessage = "The username must be 3 characters long.", MinimumLength = 3)]
    public string UserName { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = null!;

    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Mobile { get; set; }
    public int RoleId { get; set; }
    public IFormFile? ProfilePicture { get; set; }
}