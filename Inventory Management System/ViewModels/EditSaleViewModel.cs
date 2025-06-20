using Inventory_Management_System.Enums;

namespace Inventory_Management_System.ViewModels;

public class EditSaleViewModel
{
    public int Id { get; set; }
    public string Reference { get; set; } = null!;
    public decimal? DiscountAmount { get; set; }
    public DateTime SaleDate { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? AppUserId { get; set; } 

    public List<SaleItemInput> SaleItems { get; set; } = new();
    public class SaleItemInput
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string ProductImageFileName { get; set; } = "product-default.png";
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
