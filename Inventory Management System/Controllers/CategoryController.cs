using Inventory_Management_System.Contexts;
using Inventory_Management_System.Models;
using Inventory_Management_System.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Management_System.Controllers;

public class CategoryController : Controller
{
    private readonly AppDbContext _context;

    public CategoryController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var categories = _context.Categories
            .Where(c => !c.IsDeleted)
            .OrderByDescending(c => c.CreatedDate)
            .ToList();

        return View(categories);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CategoryCreateViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        bool codeExists = await _context.Categories
            .AnyAsync(c => c.Code == model.Code && !c.IsDeleted);

        if (codeExists)
        {
            ModelState.AddModelError("Code", "This code already exists.");
            return View(model);
        }

        var category = new Category
        {
            Name = model.Name,
            Description = model.Description,
            Code = model.Code,
            CreatedDate = DateTime.Now,
            UpdatedDate = DateTime.Now,
            IsDeleted = false
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Update(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null || category.IsDeleted)
            return NotFound();

        var viewModel = new CategoryUpdateViewModel
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            Code = category.Code,
            UpdatedDate = category.UpdatedDate
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Update(CategoryUpdateViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var category = await _context.Categories.FindAsync(model.Id);
        if (category == null || category.IsDeleted)
            return NotFound();

        bool codeExists = await _context.Categories
            .AnyAsync(c => c.Id != model.Id && c.Code == model.Code && !c.IsDeleted);

        if (codeExists)
        {
            ModelState.AddModelError("Code", "This category code already exists.");
            return View(model);
        }

        category.Name = model.Name;
        category.Description = model.Description;
        category.Code = model.Code;
        category.UpdatedDate = DateTime.Now;

        _context.Categories.Update(category);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (category == null || category.IsDeleted)
            return NotFound();

        category.IsDeleted = true;
        category.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }
}
