using Inventory_Management_System.DTOs;

namespace Inventory_Management_System.ViewModels;


public class DashboardViewModel
{
    public decimal MonthlyRevenue { get; set; }
    public decimal TotalProfit { get; set; }
    public decimal TotalSaleAmount { get; set; }
    public decimal TotalPurchaseAmount { get; set; }

    public int SalesCount { get; set; }
    public int PurchasesCount { get; set; }
    public int SoldItemsCount { get; set; }
    public int TotalProductsCount { get; set; }

    public List<ProductDto> RecentlyAddedProducts { get; set; }
    public List<ProductDto> OutOfStockProducts { get; set; }
}
