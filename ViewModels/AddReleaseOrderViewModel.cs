using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Inventory_Management_System.Models;

namespace Inventory_Management_System.ViewModels
{
    public class AddReleaseOrderViewModel
    {
        // For editing, you might need an Id property
        public int Id { get; set; }

        [Required(ErrorMessage = "Order number is required")]
        public string OrderNumber { get; set; }

        [Required(ErrorMessage = "Order date is required")]
        public DateTime OrderDate { get; set; }

        [Required(ErrorMessage = "Please select a warehouse")]
        public int WarehouseId { get; set; }

        [Required(ErrorMessage = "Please select a supplier")]
        public int SupplierId { get; set; } // The request specifies supplier name, so we link to Supplier

        public List<ReleaseOrderItemViewModel> Items { get; set; } = new List<ReleaseOrderItemViewModel>();

        // Navigation properties for dropdowns
        public List<Warehouse> Warehouses { get; set; } = new List<Warehouse>();
        public List<Supplier> Suppliers { get; set; } = new List<Supplier>();
        public List<Product> Products { get; set; } = new List<Product>();
    }
}