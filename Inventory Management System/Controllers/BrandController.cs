using Inventory_Management_System.Contexts;
using Inventory_Management_System.Models;
using Inventory_Management_System.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Management_System.Controllers;

public class BrandController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public BrandController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public IActionResult Index()
    {
        var brands = _context.Brands
            .Where(b => !b.IsDeleted)
            .OrderByDescending(b => b.CreatedDate)
            .ToList();

        return View(brands);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(BrandCreateViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var brand = new Brand
        {
            Name = model.Name,
            Description = model.Description,
            CreatedDate = DateTime.Now,
            UpdatedDate = DateTime.Now,
            IsDeleted = false
        };

        if (model.LogoFile != null)
        {
            string fileName = Guid.NewGuid() + Path.GetExtension(model.LogoFile.FileName);
            string filePath = Path.Combine(_env.WebRootPath, "assets", "img", "brand", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.LogoFile.CopyToAsync(stream);
            }

            brand.Logo = fileName;
        }

        _context.Brands.Add(brand);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Update(int id)
    {
        var brand = await _context.Brands.FindAsync(id);
        if (brand == null || brand.IsDeleted)
            return NotFound();

        var viewModel = new BrandUpdateViewModel
        {
            Id = brand.Id,
            Name = brand.Name,
            Description = brand.Description,
            ExistingLogo = brand.Logo
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Update(BrandUpdateViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var brand = await _context.Brands.FindAsync(model.Id);
        if (brand == null || brand.IsDeleted)
            return NotFound();

        brand.Name = model.Name;
        brand.Description = model.Description;
        brand.UpdatedDate = DateTime.Now;

        if (model.LogoFile != null)
        {
            string fileName = Guid.NewGuid() + Path.GetExtension(model.LogoFile.FileName);
            string filePath = Path.Combine(_env.WebRootPath, "assets", "img", "brand", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.LogoFile.CopyToAsync(stream);
            }

            brand.Logo = fileName;
        }

        _context.Brands.Update(brand);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var brand = await _context.Brands.FindAsync(id);
        if (brand == null || brand.IsDeleted)
            return NotFound();

        brand.IsDeleted = true;
        brand.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }
}
