using Inventory_Management_System.Enums;
using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.ViewModels;

public class CreateSaleViewModel
{
    public decimal? DiscountAmount { get; set; }

    [Required(ErrorMessage = "Payment method is required.")]
    public PaymentMethod PaymentMethod { get; set; }
    public string Reference { get; set; } = null!;
    public DateTime CreatedDate { get; set; }


    [MinLength(1, ErrorMessage = "At least one product must be added.")]
    public List<SaleItemInput> SaleItems { get; set; } = new();

    public class SaleItemInput
    {
        public int ProductId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int QuantitySold { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal UnitPrice { get; set; }
    }
}
