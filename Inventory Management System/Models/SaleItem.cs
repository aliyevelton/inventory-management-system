namespace Inventory_Management_System.Models;

public class SaleItem
{
    public int Id { get; set; }

    public int SaleId { get; set; }
    public Sale Sale { get; set; } = null!;
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int QuantitySold { get; set; }

    public decimal UnitPrice { get; set; } 

    public decimal TotalPrice => UnitPrice * QuantitySold;
    public int QuantityReturned { get; set; } = 0;
}
