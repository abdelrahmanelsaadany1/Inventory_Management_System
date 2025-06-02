using System;
using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.ViewModels
{
    public class ProductTransferItemViewModel
    {
        [Required(ErrorMessage = "Product is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a product.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Supplier is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a supplier.")]
        public int SupplierId { get; set; }

        [Required(ErrorMessage = "Production Date is required.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ProductionDate { get; set; } // This will be set by JS from existing inventory

        [Required(ErrorMessage = "Expiry Period is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Expiry Period must be a non-negative number.")]
        public int ExpiryPeriodInDays { get; set; } // This will be set by JS from existing inventory

        // New property for displaying available quantity (not for binding in the form submission, but for display)
        [Display(Name = "Available")]
        public int QuantityAvailable { get; set; }
    }
}