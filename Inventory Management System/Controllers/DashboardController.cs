using Inventory_Management_System.Contexts;
using Inventory_Management_System.DTOs;
using Inventory_Management_System.Models;
using Inventory_Management_System.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Inventory_Management_System.Controllers;

public class DashboardController : Controller
{
    private readonly AppDbContext _context;
    public DashboardController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var now = DateTime.Now;
        var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
        var firstDayOfNextMonth = firstDayOfMonth.AddMonths(1);

        var model = new DashboardViewModel
        {
            MonthlyRevenue = await _context.Sales
            .Where(s => s.CreatedDate >= firstDayOfMonth && s.CreatedDate < firstDayOfNextMonth)
            .SumAsync(s => s.TotalPrice),

            TotalProfit = await _context.Sales
                .SumAsync(s => s.TotalPrice),

            TotalSaleAmount = await _context.Sales.SumAsync(s => s.TotalPrice),
            TotalPurchaseAmount = await _context.Purchases.SumAsync(p => p.TotalCost),

            SalesCount = await _context.Sales.CountAsync(),
            PurchasesCount = await _context.Purchases.CountAsync(),
            SoldItemsCount = await _context.SaleItems.SumAsync(i => i.QuantitySold),
            TotalProductsCount = await _context.Products.CountAsync(),

            RecentlyAddedProducts = await _context.Products
                .Where(p => !p.IsDeleted)
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .OrderByDescending(p => p.CreatedDate)
                .Take(5)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    SKU = p.SKU,
                    Name = p.Name,
                    Price = p.Price,
                    Image = p.ProductImages.FirstOrDefault().Image,
                    BrandName = p.Brand.Name,
                    CategoryName = p.Category.Name
                }).ToListAsync(),

            OutOfStockProducts = await _context.Products
                .Where(p => p.Quantity == 0)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    SKU = p.SKU,
                    Name = p.Name,
                    Image = p.ProductImages.FirstOrDefault().Image,
                    BrandName = p.Brand.Name,
                    CategoryName = p.Category.Name
                }).ToListAsync()
        };
        var currentYear = DateTime.Now.Year;

        var salesByMonth = await _context.Sales
            .Where(s => s.CreatedDate.Year == currentYear)
            .GroupBy(s => s.CreatedDate.Month)
            .Select(g => new { Month = g.Key, TotalSales = g.Sum(s => s.TotalPrice) })
            .ToListAsync();

        var purchasesByMonth = await _context.Purchases
            .Where(p => p.PurchaseDate.Year == currentYear)
            .GroupBy(p => p.PurchaseDate.Month)
            .Select(g => new { Month = g.Key, TotalPurchases = g.Sum(p => p.TotalCost) })
            .ToListAsync();

        var salesData = new decimal[12];
        var purchaseData = new decimal[12];
        for (int i = 1; i <= 12; i++)
        {
            salesData[i - 1] = salesByMonth.FirstOrDefault(x => x.Month == i)?.TotalSales ?? 0;
            purchaseData[i - 1] = purchasesByMonth.FirstOrDefault(x => x.Month == i)?.TotalPurchases ?? 0;
        }

        ViewBag.SalesData = salesData;
        ViewBag.PurchaseData = purchaseData;

        return View(model);
    }


    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
