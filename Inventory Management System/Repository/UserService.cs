using Inventory_Management_System.Areas.Admin.ViewModels;
using Inventory_Management_System.Contexts;
using Inventory_Management_System.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Management_System.Repository;

public class UserService : IUserService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly AppDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public UserService(UserManager<AppUser> userManager, AppDbContext context, IWebHostEnvironment webHostEnvironment, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _context = context;
        _webHostEnvironment = webHostEnvironment;
        _roleManager = roleManager;
    }

    public UserViewModel GetUserById(string id)
    {
        var user = _userManager.Users
            .Include(u => u.UserRole)
            .FirstOrDefault(u => u.Id == id);
        if (user == null) return null;

        return new UserViewModel
        {
            Id = user.Id,
            ProfileImage = user.Image,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.UserRole?.Name ?? "No Role",
            Phone = user.PhoneNumber,
            Email = user.Email,
            IsActive = user.IsActive
        };
    }

    public async Task<IdentityResult> UpdateUserAsync(UserEditViewModel model)
    {
        var user = await _userManager.FindByIdAsync(model.Id);
        if (user == null)
            return IdentityResult.Failed(new IdentityError { Description = "User not found" });

        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.Email = model.Email;
        user.PhoneNumber = model.Mobile;

        // Password update
        if (!string.IsNullOrWhiteSpace(model.Password))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var passwordResult = await _userManager.ResetPasswordAsync(user, token, model.Password);
            if (!passwordResult.Succeeded)
            {
                return passwordResult; // Return password validation errors to controller
            }
        }

        // Update Identity Role
        var currentRoles = await _userManager.GetRolesAsync(user);
        if (!currentRoles.Contains(model.Role))
        {
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                var errors = removeResult.Errors.Select(e => e.Description);
                return IdentityResult.Failed(errors.Select(e => new IdentityError { Description = e }).ToArray());
            }

            string identityRoleName = model.Role switch
            {
                "Salesperson" or "Cashier" or "Purchasing Officer" or "Accountant" => "User",
                "Store Manager" => "Moderator",
                "Inventory Clerk" => "Inventory Manager",
                "Admin" => "Admin",
                _ => "User"
            };

            var roleExists = await _roleManager.RoleExistsAsync(identityRoleName);
            if (!roleExists)
            {
                return IdentityResult.Failed(new IdentityError { Description = $"Role '{identityRoleName}' does not exist." });
            }

            var addResult = await _userManager.AddToRoleAsync(user, identityRoleName);
            if (!addResult.Succeeded)
            {
                var errors = addResult.Errors.Select(e => e.Description);
                return IdentityResult.Failed(errors.Select(e => new IdentityError { Description = e }).ToArray());
            }
        }

        // Update Custom Role (UserRoleId) by role name
        var selectedCustomRole = await _context.CustomUserRoles.FirstOrDefaultAsync(r => r.Name == model.Role);
        if (selectedCustomRole == null)
            return IdentityResult.Failed(new IdentityError { Description = "Selected role not found in CustomUserRoles table." });

        user.UserRoleId = selectedCustomRole.Id;

        // Handle image removal
        if (model.RemoveProfileImage)
        {
            if (!string.IsNullOrEmpty(user.Image))
            {
                var oldPath = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "img", "profiles", user.Image);
                if (System.IO.File.Exists(oldPath))
                    System.IO.File.Delete(oldPath);
            }

            user.Image = null;
        }

        // Handle new image upload
        if (model.NewProfileImage != null && model.NewProfileImage.Length > 0)
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "img", "profiles");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.NewProfileImage.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.NewProfileImage.CopyToAsync(stream);
            }

            user.Image = uniqueFileName;
        }

        var updateResult = await _userManager.UpdateAsync(user);
        return updateResult;
    }



    public async Task UpdateUserStatus(UserViewModel userViewModel, ToggleStatusViewModel toggleStatusViewModel)
    {
        var user = await _userManager.FindByIdAsync(userViewModel.Id);
        if (user == null) throw new Exception("User not found");

        user.IsActive = toggleStatusViewModel.IsActive;
        await _userManager.UpdateAsync(user);
    }
}
