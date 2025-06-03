// ViewModels/WarehouseActivityReportRequestViewModel.cs
using Microsoft.AspNetCore.Mvc.Rendering; 
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
        public DateTime? StartDate { get; set; } 

        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        
        public SelectList? AllWarehouses { get; set; }

       
        public List<WarehouseActivityReportViewModel>? ReportData { get; set; }
    }
}