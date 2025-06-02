using Inventory_Management_System.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.ViewModels
{
    public class AddProductTransferViewModel
    {
        public int Id { get; set; } // For potential future editing (though not implemented in the controller yet)

        [Required(ErrorMessage = "Transfer Number is required.")]
        [StringLength(50, ErrorMessage = "Transfer Number cannot exceed 50 characters.")]
        [Display(Name = "Transfer Number")]
        public string TransferNumber { get; set; }

        [Required(ErrorMessage = "Transfer Date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Transfer Date")]
        public DateTime TransferDate { get; set; }

        [Required(ErrorMessage = "Source Warehouse is required.")]
        [Display(Name = "From Warehouse")]
        public int SourceWarehouseId { get; set; }

        [Required(ErrorMessage = "Destination Warehouse is required.")]
        [Display(Name = "To Warehouse")]
        public int DestinationWarehouseId { get; set; }

        // Collection for transfer items
        [Display(Name = "Transfer Items")]
        public List<ProductTransferItemViewModel> Items { get; set; }

        // Dropdown data for the view
        public List<Warehouse> Warehouses { get; set; }
        // The Products and Suppliers lists were removed as they are loaded dynamically via AJAX.
    }
}