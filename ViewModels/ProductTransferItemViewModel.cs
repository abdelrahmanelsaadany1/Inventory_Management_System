using System;
using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.ViewModels
{
    public class ProductTransferItemViewModel
    {
        public int Index { get; set; }

        // Removed all validation attributes for debugging
        public int ProductId { get; set; }
        public int SupplierId { get; set; }
        public DateTime ProductionDate { get; set; }

        // Removed Display, Required, and Range attributes
        public int Quantity { get; set; }

        public int ExpiryPeriodInDays { get; set; }
    }
}