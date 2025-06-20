using Inventory_Management_System.Contexts;
using Inventory_Management_System.Models;
using Inventory_Management_System.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Management_System.Controllers;

public class ProductController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ProductController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<IActionResult> Index(int? categoryId, int? brandId)
    {
        var productsQuery = _context.Products
        .Where(p => !p.IsDeleted)
        .Include(p => p.Category)
        .Include(p => p.Brand)
        .Include(p => p.ProductImages)
        .AsQueryable();


        if (categoryId.HasValue)
        {
            productsQuery = productsQuery.Where(p => p.CategoryId == categoryId.Value);
        }

        if (brandId.HasValue)
        {
            productsQuery = productsQuery.Where(p => p.BrandId == brandId.Value);
        }

        var products = await productsQuery
            .ToListAsync();

        ViewBag.Categories = new SelectList(
            await _context.Categories.Where(c => !c.IsDeleted).ToListAsync(),
            "Id",
            "Name",
            categoryId);

        ViewBag.Brands = new SelectList(
            await _context.Brands.Where(b => !b.IsDeleted).ToListAsync(),
            "Id",
            "Name",
            brandId);

        ViewBag.SelectedCategory = categoryId;
        ViewBag.SelectedBrand = brandId;

        return View(products);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var product = await _context.Products
            .Where(p => !p.IsDeleted)
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.ProductImages)
            .Where(p => p.Id == id)
            .Select(p => new ProductDetailViewModel
            {
                Name = p.Name,
                CategoryName = p.Category.Name,
                BrandName = p.Brand != null ? p.Brand.Name : "None",
                SKU = p.SKU,
                Quantity = p.Quantity,
                Price = p.Price,
                DiscountedPrice = p.DiscountedPrice,
                IsActive = p.IsActive,
                Description = p.Description,
                CreatedDate = p.CreatedDate,
                Images = p.ProductImages.Select(img => new ProductImageViewModel
                {
                    ImageName = img.Image,
                    SizeKb = new FileInfo(Path.Combine(_webHostEnvironment.WebRootPath, "assets/img/product", img.Image)).Length / 1024
                }).ToList()
            })
            .FirstOrDefaultAsync();

        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }


    [Authorize(Roles = "Admin,Manager,Inventory Manager")]
    public IActionResult Create()
    {
        ViewBag.Categories = new SelectList(_context.Categories.Where(c => !c.IsDeleted), "Id", "Name");
        ViewBag.Brands = new SelectList(_context.Brands.Where(b => !b.IsDeleted), "Id", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager,Inventory Manager")]
    public async Task<IActionResult> Create(ProductCreateViewModel model)
    {
        ViewBag.Categories = new SelectList(_context.Categories.Where(c => !c.IsDeleted), "Id", "Name");
        ViewBag.Brands = new SelectList(_context.Brands.Where(b => !b.IsDeleted), "Id", "Name");

        if (!ModelState.IsValid)
            return View(model);

        var product = new Product
        {
            Name = model.Name,
            Description = model.Description,
            Price = model.Price,
            DiscountedPrice = model.DiscountedPrice,
            SKU = model.SKU,
            Quantity = 0,
            CategoryId = model.CategoryId,
            BrandId = model.BrandId,
            IsActive = true,
            IsDeleted = false,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
            ProductImages = new List<ProductImage>()
        };

        if (model.ImageFiles != null)
        {
            foreach (var file in model.ImageFiles)
            {
                if (file.Length > 0)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    var savePath = Path.Combine(_webHostEnvironment.WebRootPath, "assets/img/product", fileName);

                    using (var stream = new FileStream(savePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    product.ProductImages.Add(new ProductImage
                    {
                        Image = fileName,
                        CreatedDate = DateTime.UtcNow 
                    });
                }
            }

        }

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [Authorize(Roles = "Admin,Manager,Inventory Manager")]
    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        var product = await _context.Products
            .Where(p => !p.IsDeleted)
            .Include(p => p.ProductImages)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null) return NotFound();

        var model = new ProductUpdateViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            DiscountedPrice = product.DiscountedPrice,
            SKU = product.SKU,
            CategoryId = product.CategoryId,
            BrandId = product.BrandId,
            IsActive = product.IsActive,
            ExistingImageNames = product.ProductImages?.Select(p => p.Image).ToList(),
            ExistingImageIds = product.ProductImages?.Select(p => p.Id).ToList()
        };

        ViewBag.Categories = new SelectList(await _context.Categories.Where(c => !c.IsDeleted).ToListAsync(), "Id", "Name", model.CategoryId);
        ViewBag.Brands = new SelectList(await _context.Brands.Where(b => !b.IsDeleted).ToListAsync(), "Id", "Name", model.BrandId);

        return View(model);
    }



    [HttpPost]
    [Authorize(Roles = "Admin,Manager,Inventory Manager")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(ProductUpdateViewModel model)
    {
        var product = await _context.Products
            .Where(p => !p.IsDeleted)
            .Include(p => p.ProductImages)
            .FirstOrDefaultAsync(p => p.Id == model.Id);

        if (product == null) return NotFound();

        if (!ModelState.IsValid)
        {
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", model.CategoryId);
            ViewBag.Brands = new SelectList(await _context.Brands.ToListAsync(), "Id", "Name", model.BrandId);

            model.ExistingImageNames = product.ProductImages?.Select(p => p.Image).ToList();
            model.ExistingImageIds = product.ProductImages?.Select(p => p.Id).ToList();

            return View(model); 
        }

        bool isDuplicateSKU = await _context.Products.Where(p => !p.IsDeleted).AnyAsync(p => p.SKU == model.SKU && p.Id != model.Id);

        if (isDuplicateSKU)
        {
            ModelState.AddModelError("SKU", "This SKU already exists. Please choose a different SKU.");

            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", model.CategoryId);
            ViewBag.Brands = new SelectList(await _context.Brands.ToListAsync(), "Id", "Name", model.BrandId);

            model.ExistingImageNames = product?.ProductImages?.Select(p => p.Image).ToList();
            model.ExistingImageIds = product?.ProductImages?.Select(p => p.Id).ToList();

            return View(model);
        }

        product.Name = model.Name;
        product.Description = model.Description;
        product.Price = model.Price;
        product.DiscountedPrice = model.DiscountedPrice;
        product.SKU = model.SKU;
        product.CategoryId = model.CategoryId;
        product.BrandId = model.BrandId;
        product.IsActive = model.IsActive;
        product.UpdatedDate = DateTime.Now;

        if (model.ProductImages != null && model.ProductImages.Any())
        {
            foreach (var file in model.ProductImages)
            {
                if (file.Length > 0)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "assets/img/product", fileName);

                    Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    product.ProductImages ??= new List<ProductImage>();
                    product.ProductImages.Add(new ProductImage
                    {
                        Image = fileName,
                        CreatedDate = DateTime.Now
                    });
                }
            }
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager,Inventory Manager")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteImage(int id)
    {
        var image = await _context.ProductImages.FindAsync(id);
        if (image == null) return NotFound();

        var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "assets/img/product", image.Image);
        if (System.IO.File.Exists(imagePath))
        {
            System.IO.File.Delete(imagePath);
        }

        // Remove from database
        _context.ProductImages.Remove(image);
        await _context.SaveChangesAsync();

        return RedirectToAction("Update", new { id = image.ProductId });
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager,Inventory Manager")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.Where(p => !p.IsDeleted).FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return NotFound();

        product.IsDeleted = true;
        product.UpdatedDate = DateTime.UtcNow; 

        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpGet]
    public IActionResult SearchProducts(string term)
    {
        var products = _context.Products
            .Where(p => p.Name.Contains(term))
            .Where(p => !p.IsDeleted)
            .Select(p => new
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
            })
            .Take(10)
            .ToList();

        return Json(products);
    }
}
