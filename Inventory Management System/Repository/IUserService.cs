using Inventory_Management_System.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Inventory_Management_System.Repository;

public interface IUserService
{
    UserViewModel GetUserById(string id);
    Task<IdentityResult> UpdateUserAsync(UserEditViewModel model);
    Task UpdateUserStatus(UserViewModel userViewModel, ToggleStatusViewModel toggleStatusViewModel);
}

