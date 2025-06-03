using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.ViewModels
{
    public class ProductTransferViewModel
    {
        [Required(ErrorMessage = "Transfer Date is required")]
        [Display(Name = "Transfer Date")]
        public DateTime TransferDate { get; set; }

        [Required(ErrorMessage = "Source Warehouse is required")]
        [Display(Name = "Source Warehouse")]
        public int SourceWarehouseId { get; set; }

        [Required(ErrorMessage = "Destination Warehouse is required")]
        [Display(Name = "Destination Warehouse")]
        public int DestinationWarehouseId { get; set; }

        public List<ProductTransferItemViewModel> Items { get; set; } = new List<ProductTransferItemViewModel>();
    }

    public class ProductTransferItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }

        public int SupplierId { get; set; }
        public string SupplierName { get; set; }

        public DateTime ProductionDate { get; set; }
        public int ExpiryPeriodInDays { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero")]
        public int Quantity { get; set; }

        public int AvailableQuantity { get; set; }
    }
}