using Inventory_Management_System.Areas.Admin.ViewModels;
using Inventory_Management_System.Contexts;
using Inventory_Management_System.Models;
using Inventory_Management_System.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Management_System.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin, Moderator")]
public class UserController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IUserService _userService;

    public UserController(AppDbContext context, UserManager<AppUser> userManager, IWebHostEnvironment webHostEnvironment, IUserService userService)
    {
        _context = context;
        _userManager = userManager;
        _webHostEnvironment = webHostEnvironment;
        _userService = userService;
    }

    public async Task<IActionResult> Index(bool? isActive)
    {
        var usersQuery = _context.Users.AsQueryable();

        if (isActive.HasValue)
        {
            usersQuery = usersQuery.Where(u => u.IsActive == isActive.Value);
        }

        var users = await usersQuery
            .Select(u => new UserViewModel
            {
                Id = u.Id,
                ProfileImage = u.Image,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Role = u.UserRole.Name,
                Phone = u.PhoneNumber,
                Email = u.Email,
                IsActive = u.IsActive
            })
            .ToListAsync();

        return View(users);
    }


    public async Task<IActionResult> Create()
    {
        var roles = await _context.CustomUserRoles.ToListAsync();
        ViewBag.Roles = roles;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserCreateViewModel dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Roles = await _context.CustomUserRoles.ToListAsync();
            return View(dto);
        }

        var customRole = await _context.CustomUserRoles.FindAsync(dto.RoleId);
        if (customRole == null)
        {
            ModelState.AddModelError("RoleId", "Invalid role selected.");
            ViewBag.Roles = await _context.CustomUserRoles.ToListAsync();
            return View(dto);
        }

        string imageFileName = null;

        if (dto.ProfilePicture != null && dto.ProfilePicture.Length > 0)
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "img", "profiles");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            imageFileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ProfilePicture.FileName);

            var filePath = Path.Combine(uploadsFolder, imageFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.ProfilePicture.CopyToAsync(stream);
            }
        }

        var roles = await _context.CustomUserRoles.ToListAsync();
        ViewBag.Roles = roles;

        bool hasError = false;

        if (await _userManager.FindByNameAsync(dto.UserName) is not null)
        {
            ModelState.AddModelError("UserName", "Username is already taken.");
            hasError = true;
        }

        if (await _userManager.FindByEmailAsync(dto.Email) is not null)
        {
            ModelState.AddModelError("Email", "Email is already taken.");
            hasError = true;
        }

        if (hasError)
            return View(dto);


        var user = new AppUser
        {
            UserName = dto.UserName,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhoneNumber = dto.Mobile,
            Image = imageFileName,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
            IsActive = true, 
            UserRoleId = dto.RoleId
        };


        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            ViewBag.Roles = await _context.CustomUserRoles.ToListAsync();
            return View(dto);
        }

        string identityRoleName = customRole.Name switch
        {
            "Salesperson" or "Cashier" or "Purchasing Officer" or "Accountant" => "User",
            "Store Manager" => "Moderator",
            "Inventory Clerk" => "Inventory Manager",
            "Admin" => "Admin",
            _ => "User"
        };

        await _userManager.AddToRoleAsync(user, identityRoleName);

        return RedirectToAction("Index");
    }


    public async Task<IActionResult> Update(string id)
    {
        var user = _userService.GetUserById(id);

        var roles = await _context.CustomUserRoles.ToListAsync();

        var model = new UserEditViewModel
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Mobile = user.Phone,
            CurrentProfileImage = user.ProfileImage,
            Role = user.Role,
            RoleList = roles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Name,
                Selected = r.Name == user.Role
            }).ToList()
        };


        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(UserEditViewModel model)
    {
        if (!string.IsNullOrEmpty(model.Password) || !string.IsNullOrEmpty(model.ConfirmPassword))
        {
            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Passwords do not match.");
            }
        }

        if (!ModelState.IsValid)
        {
            // Repopulate RoleList
            var roles = await _context.CustomUserRoles.ToListAsync();
            model.RoleList = roles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Name,
                Selected = r.Name == model.Role
            }).ToList();

            // Load current profile image from DB (or default)
            var user = await _context.Users.FindAsync(model.Id);
            model.CurrentProfileImage = user?.Image ?? "default.png";

            return View(model);
        }

        var updateResult = await _userService.UpdateUserAsync(model);

        if (!updateResult.Succeeded)
        {
            foreach (var error in updateResult.Errors)
            {
                ModelState.AddModelError("Password", error.Description);
            }

            // Repopulate RoleList
            var roles = await _context.CustomUserRoles.ToListAsync();
            model.RoleList = roles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Name,
                Selected = r.Name == model.Role
            }).ToList();

            var user = await _context.Users.FindAsync(model.Id);
            model.CurrentProfileImage = user?.Image ?? "default.png";

            return View(model);
        }

        TempData["Success"] = "User updated successfully!";
        return RedirectToAction("Index");
    }


    [HttpPost]
    [Route("Admin/User/ToggleStatus")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> ToggleStatus([FromBody] ToggleStatusViewModel vm)
    {
        if (vm == null || string.IsNullOrEmpty(vm.Id))
        {
            return BadRequest("Invalid user data.");
        }

        var user = _userService.GetUserById(vm.Id);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        try
        {
            await _userService.UpdateUserStatus(user, vm);
            return Ok(new { success = true, message = "User status updated successfully." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

}
