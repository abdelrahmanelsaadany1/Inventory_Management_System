// ViewModels/WarehouseActivityReportRequestViewModel.cs
using Microsoft.AspNetCore.Mvc.Rendering; // For SelectList
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.ViewModels
{
    public class WarehouseActivityReportRequestViewModel
    {
        [Display(Name = "Select Warehouse")]
        [Required(ErrorMessage = "Please select a warehouse.")]
        public int SelectedWarehouseId { get; set; }

        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; } // CHANGED TO NULLABLE DATETIME?

        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; } // CHANGED TO NULLABLE DATETIME?

        // Property for the warehouse dropdown (populated by controller)
        public SelectList? AllWarehouses { get; set; }

        // This will hold the actual report data after generation
        public List<WarehouseActivityReportViewModel>? ReportData { get; set; }
    }
}