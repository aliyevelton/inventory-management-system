using Inventory_Management_System.Contexts;
using Inventory_Management_System.Models;
using Inventory_Management_System.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static Inventory_Management_System.ViewModels.CreatePurchaseViewModel;

namespace Inventory_Management_System.Controllers;

[Authorize]
public class PurchaseController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public PurchaseController(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        var purchases = _context.Purchases
            .Include(p => p.AppUser)
            .ToList();
        return View(purchases);
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.Products = _context.Products
            .Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Name
            })
            .ToList();

        var model = new CreatePurchaseViewModel();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePurchaseViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Products = _context.Products
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name
                })
                .ToList();
            return View(model);
        }

        var userId = _userManager.GetUserId(User);
        if (userId == null)
            return Unauthorized();

        var purchase = new Purchase
        {
            AppUserId = userId,
            SupplierName = model.SupplierName,
            PurchaseDate = model.PurchaseDate,
            Reference = model.Reference,
            PurchaseItems = model.PurchaseItems.Select(item => new PurchaseItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitCost = item.UnitCost
            }).ToList()
        };

        // Calculate total cost
        purchase.TotalCost = purchase.PurchaseItems.Sum(pi => pi.Quantity * pi.UnitCost);

        bool referenceExists = await _context.Purchases.AnyAsync(p => p.Reference == model.Reference);
        if (referenceExists)
        {
            ModelState.AddModelError("Reference", "This reference already exists. Please enter a unique reference.");

            ViewBag.Products = _context.Products
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name
                })
                .ToList();

            return View(model);
        }

        _context.Purchases.Add(purchase);

        // Update product quantities
        foreach (var item in purchase.PurchaseItems)
        {
            var product = await _context.Products.FindAsync(item.ProductId);
            if (product == null)
                return BadRequest($"Product with ID {item.ProductId} not found.");

            product.Quantity += item.Quantity; // Increase stock on purchase
        }

        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Details(int id)
    {
        var purchase = _context.Purchases
            .Include(p => p.AppUser)
            .Include(p => p.PurchaseItems)
                .ThenInclude(pi => pi.Product)
                .ThenInclude(p => p.ProductImages)
            .FirstOrDefault(p => p.Id == id);

        if (purchase == null)
            return NotFound();

        var model = new PurchaseDetailsViewModel
        {
            Id = purchase.Id,
            SupplierName = purchase.SupplierName,
            PurchaseDate = purchase.PurchaseDate,
            Reference = purchase.Reference,
            TotalCost = purchase.TotalCost,
            AppUserName = purchase.AppUser?.UserName,
            PurchaseItems = purchase.PurchaseItems.Select(pi => new PurchaseDetailsViewModel.PurchaseItemDetails
            {
                ProductName = pi.Product.Name,
                ProductImageFileName = pi.Product.ProductImages?.FirstOrDefault()?.Image ?? "product-default.png",
                Quantity = pi.Quantity,
                UnitCost = pi.UnitCost
            }).ToList()
        };

        return View(model);
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var purchase = _context.Purchases
            .Include(p => p.PurchaseItems)
                .ThenInclude(pi => pi.Product)
                .ThenInclude(p => p.ProductImages)
            .Include(p => p.AppUser)
            .FirstOrDefault(p => p.Id == id);

        if (purchase == null) return NotFound();

        var viewModel = new EditPurchaseViewModel
        {
            Id = purchase.Id,
            SupplierName = purchase.SupplierName,
            Reference = purchase.Reference,
            PurchaseDate = purchase.PurchaseDate,
            AppUserId = purchase.AppUserId,
            PurchaseItems = purchase.PurchaseItems.Select(pi => new EditPurchaseViewModel.PurchaseItemInput
            {
                ProductId = pi.ProductId,
                ProductName = pi.Product.Name,
                ProductImageFileName = pi.Product.ProductImages?.FirstOrDefault()?.Image ?? "product-default.png",
                Quantity = pi.Quantity,
                UnitCost = pi.UnitCost
            }).ToList()
        };


        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(EditPurchaseViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var purchase = _context.Purchases
            .Include(p => p.PurchaseItems)
            .FirstOrDefault(p => p.Id == model.Id);

        if (purchase == null) return NotFound();

        purchase.SupplierName = model.SupplierName;
        purchase.PurchaseDate = model.PurchaseDate;
        purchase.Reference = model.Reference;
        purchase.TotalCost = model.PurchaseItems.Sum(pi => pi.Quantity * pi.UnitCost);

        _context.PurchaseItems.RemoveRange(purchase.PurchaseItems);

        purchase.PurchaseItems = model.PurchaseItems.Select(pi => new PurchaseItem
        {
            ProductId = pi.ProductId,
            Quantity = pi.Quantity,
            UnitCost = pi.UnitCost
        }).ToList();

        _context.SaveChanges();

        return RedirectToAction("Details", new { id = purchase.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var purchase = await _context.Purchases
            .Include(p => p.PurchaseItems)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (purchase == null)
            return NotFound();

        return View(purchase);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var purchase = await _context.Purchases
            .Include(p => p.PurchaseItems)
            .ThenInclude(pi => pi.Product)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (purchase == null)
        {
            return NotFound();
        }

        // Decrease the stock quantity for each product in the purchase
        foreach (var item in purchase.PurchaseItems)
        {
            item.Product.Quantity -= item.Quantity;

            if (item.Product.Quantity < 0)
            {
                item.Product.Quantity = 0;
            }
        }

        _context.Purchases.Remove(purchase);

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Purchase deleted successfully!";

        return RedirectToAction(nameof(Index));
    }

}