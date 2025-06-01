using System;
using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.ViewModels
{
    public class SupplyOrderItemViewModel
    {
        [Required(ErrorMessage = "Please select a product")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid product")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Production date is required")]
        public DateTime ProductionDate { get; set; }

        [Required(ErrorMessage = "Expiry period is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Expiry period must be greater than 0")]
        public int ExpiryPeriodInDays { get; set; }
    }
}
