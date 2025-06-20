using Microsoft.AspNetCore.Identity;

namespace Inventory_Management_System.Models;

public class AppUser : IdentityUser
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Image { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public bool IsActive { get; set; }
    public int? UserRoleId { get; set; }
    public UserRole? UserRole { get; set; }
}
