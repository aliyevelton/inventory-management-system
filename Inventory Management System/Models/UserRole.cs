namespace Inventory_Management_System.Models;

public class UserRole
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<AppUser> Users { get; set; } = new List<AppUser>();
}
