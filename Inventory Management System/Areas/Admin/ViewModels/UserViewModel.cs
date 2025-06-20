namespace Inventory_Management_System.Areas.Admin.ViewModels;

public class UserViewModel
{
    public string Id { get; set; }
    public string ProfileImage { get; set; }  
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Role { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public bool IsActive { get; set; }
}

