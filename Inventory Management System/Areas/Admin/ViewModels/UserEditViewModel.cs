using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.Areas.Admin.ViewModels;

public class UserEditViewModel
{
    public string Id { get; set; }
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required, EmailAddress]
    public string Email { get; set; }

    [DataType(DataType.Password)]
    public string? Password { get; set; }
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string? ConfirmPassword { get; set; }

    [Phone]
    public string Mobile { get; set; }

    public string? CurrentProfileImage { get; set; } 

    public IFormFile? NewProfileImage { get; set; }
    public bool RemoveProfileImage { get; set; }

    [Required]
    public string Role { get; set; }

    [BindNever]
    public List<SelectListItem> RoleList { get; set; } = new();
}
