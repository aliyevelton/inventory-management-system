using Inventory_Management_System.Enums;

namespace Inventory_Management_System.ViewModels;

public class SaleDetailsViewModel
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? AppUserName { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal? DiscountAmount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string Reference { get; set; } = string.Empty;

    public List<SaleItemDetails> SaleItems { get; set; } = new();

    public class SaleItemDetails
    {
        public string ProductName { get; set; } = string.Empty;
        public string ProductImageFileName { get; set; } = "default.png"; // Default image if none exists
        public int QuantitySold { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal => QuantitySold * UnitPrice;
    }
}