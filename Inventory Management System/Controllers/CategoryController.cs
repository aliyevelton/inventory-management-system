using Inventory_Management_System.Contexts;
using Inventory_Management_System.Models;
using Inventory_Management_System.ViewModels;
using Microsoft.AspNetCore.Mvc;

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
        return View();
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

    public IActionResult Update()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Update(CategoryUpdateViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var category = await _context.Categories.FindAsync(model.Id);
        if (category == null)
            return NotFound();

        category.Name = model.Name;
        category.Description = model.Description;
        category.Code = model.Code;
        category.UpdatedDate = DateTime.Now;

        _context.Categories.Update(category);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
