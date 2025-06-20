using Inventory_Management_System.Contexts;
using Inventory_Management_System.Enums;
using Inventory_Management_System.Models;
using Inventory_Management_System.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static Inventory_Management_System.ViewModels.CreateSaleViewModel;

namespace Inventory_Management_System.Controllers;

public class SalesController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    public SalesController(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        var sales = _context.Sales
            .Include(s => s.AppUser)
            .ToList(); 
        return View(sales);
    }

    [HttpGet]
    [Authorize]
    public IActionResult Create()
    {
        var model = new CreateSaleViewModel
        {
            CreatedDate = DateTime.Now,
            SaleItems = new List<CreateSaleViewModel.SaleItemInput>
        {
            new CreateSaleViewModel.SaleItemInput()
        }
        };

        var paymentMethods = Enum.GetValues(typeof(PaymentMethod))
            .Cast<PaymentMethod>()
            .Select(pm => new SelectListItem
            {
                Value = ((int)pm).ToString(),
                Text = pm.ToString()
            })
            .ToList();  

        ViewBag.PaymentMethods = new SelectList(paymentMethods, "Value", "Text");

        ViewBag.Products = _context.Products
            .Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Name
            })
            .ToList();

        return View(model);
    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Create(CreateSaleViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var paymentMethods = Enum.GetValues(typeof(PaymentMethod))
            .Cast<PaymentMethod>()
            .Select(pm => new SelectListItem
            {
                Value = ((int)pm).ToString(),
                Text = pm.ToString()
            })
            .ToList();

            ViewBag.PaymentMethods = new SelectList(paymentMethods, "Value", "Text");

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

        var sale = new Sale
        {
            AppUserId = userId,
            DiscountAmount = model.DiscountAmount,
            PaymentMethod = model.PaymentMethod,
            Reference = model.Reference,
            CreatedDate = model.CreatedDate,
            UpdatedDate = DateTime.UtcNow,
            SaleItems = model.SaleItems.Select(item => new SaleItem
            {
                ProductId = item.ProductId,
                QuantitySold = item.QuantitySold,
                UnitPrice = item.UnitPrice
            }).ToList()
        };

        sale.TotalPrice = sale.SaleItems.Sum(x => x.UnitPrice * x.QuantitySold) - (model.DiscountAmount ?? 0);

        bool referenceExists = await _context.Sales.AnyAsync(s => s.Reference == model.Reference);
        if (referenceExists)
        {
            ModelState.AddModelError("Reference", "This reference already exists. Please enter a unique reference.");

            var paymentMethods = Enum.GetValues(typeof(PaymentMethod))
                .Cast<PaymentMethod>()
                .Select(pm => new SelectListItem
                {
                    Value = ((int)pm).ToString(),
                    Text = pm.ToString()
                })
                .ToList();

            ViewBag.PaymentMethods = new SelectList(paymentMethods, "Value", "Text");

            ViewBag.Products = _context.Products
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name
                })
                .ToList();

            return View(model);
        }

        _context.Sales.Add(sale);

        foreach (var item in sale.SaleItems)
        {
            var product = await _context.Products.FindAsync(item.ProductId);
            if (product == null)
                return BadRequest($"Product with ID {item.ProductId} not found.");

            if (product.Quantity < item.QuantitySold)
                return BadRequest($"Not enough stock for product {product.Name}.");

            product.Quantity -= item.QuantitySold;
        }

        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [HttpGet]
    [Authorize]
    public IActionResult Details(int id)
    {
        var sale = _context.Sales
            .Include(s => s.AppUser)
            .Include(s => s.SaleItems)
                .ThenInclude(si => si.Product)
                .ThenInclude(p => p.ProductImages)
            .FirstOrDefault(s => s.Id == id);

        if (sale == null)
            return NotFound();

        var model = new SaleDetailsViewModel
        {
            Id = sale.Id,
            CreatedDate = sale.CreatedDate,
            AppUserName = sale.AppUser?.UserName,
            TotalPrice = sale.TotalPrice,
            DiscountAmount = sale.DiscountAmount,
            PaymentMethod = sale.PaymentMethod,
            Reference = sale.Reference,
            SaleItems = sale.SaleItems.Select(si => new SaleDetailsViewModel.SaleItemDetails
            {
                ProductName = si.Product.Name,
                ProductImageFileName = si.Product.ProductImages?.FirstOrDefault()?.Image ?? "product-default.png",
                QuantitySold = si.QuantitySold,
                UnitPrice = si.UnitPrice
            }).ToList()
        };

        return View(model);
    }

    public IActionResult Edit(int id)
    {
        var sale = _context.Sales
            .Include(s => s.SaleItems)
                .ThenInclude(si => si.Product)
                .ThenInclude(p => p.ProductImages)
            .Include(s => s.AppUser)
            .FirstOrDefault(s => s.Id == id);

        if (sale == null) return NotFound();

        var viewModel = new EditSaleViewModel
        {
            Id = sale.Id,
            AppUserId = sale.AppUserId,
            DiscountAmount = sale.DiscountAmount,
            Reference = sale.Reference,
            PaymentMethod = sale.PaymentMethod,
            SaleDate = sale.CreatedDate,
            SaleItems = sale.SaleItems.Select(si => new EditSaleViewModel.SaleItemInput
            {
                ProductId = si.ProductId,
                ProductName = si.Product.Name,
                ProductImageFileName = si.Product.ProductImages?.FirstOrDefault()?.Image ?? "product-default.png",
                Quantity = si.QuantitySold,
                UnitPrice = si.UnitPrice
            }).ToList()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(EditSaleViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var sale = _context.Sales
            .Include(s => s.SaleItems)
            .FirstOrDefault(s => s.Id == model.Id);

        if (sale == null) return NotFound();

        sale.Reference = model.Reference;
        sale.DiscountAmount = model.DiscountAmount;
        sale.PaymentMethod = model.PaymentMethod;
        sale.CreatedDate = model.SaleDate;
        sale.UpdatedDate = DateTime.UtcNow;
        sale.TotalPrice = model.SaleItems.Sum(x => x.UnitPrice * x.Quantity) - (model.DiscountAmount ?? 0);

        _context.SaleItems.RemoveRange(sale.SaleItems);

        sale.SaleItems = model.SaleItems.Select(si => new SaleItem
        {
            ProductId = si.ProductId,
            QuantitySold = si.Quantity,
            UnitPrice = si.UnitPrice
        }).ToList();

        _context.SaveChanges();

        return RedirectToAction("Details", new { id = sale.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var sale = await _context.Sales
            .Include(s => s.SaleItems)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (sale == null)
            return NotFound();

        return View(sale);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var sale = await _context.Sales
            .Include(s => s.SaleItems)
            .ThenInclude(si => si.Product)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (sale == null)
        {
            return NotFound();
        }

        // Increase the stock quantity for each product in the sale
        foreach (var item in sale.SaleItems)
        {
            item.Product.Quantity += item.QuantitySold;
        }

        // Remove the sale
        _context.Sales.Remove(sale);

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Sale deleted successfully!";

        return RedirectToAction(nameof(Index));
    }
}
