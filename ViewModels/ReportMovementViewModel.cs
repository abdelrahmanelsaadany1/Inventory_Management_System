using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Inventory_Management_System.Models; // Assuming models are in this namespace

namespace Inventory_Management_System.ViewModels
{
    public class ReportMovementViewModel
    {
        [Required(ErrorMessage = "Start Date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; } = DateTime.Today.AddMonths(-1); // Default to last month

        [Required(ErrorMessage = "End Date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; } = DateTime.Today; // Default to today

        [Display(Name = "Select Warehouses")]
        public List<int> SelectedWarehouseIds { get; set; } = new List<int>();

        // For dropdowns in the view
        public IEnumerable<Warehouse> AvailableWarehouses { get; set; }
    }
}