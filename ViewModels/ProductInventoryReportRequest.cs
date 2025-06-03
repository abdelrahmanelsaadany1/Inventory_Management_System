// Models/ProductInventoryReportRequestViewModel.cs
using Microsoft.AspNetCore.Mvc.Rendering; 
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.Models
{
    public class ProductInventoryReportRequestViewModel
    {
        [Display(Name = "Select Product (Optional)")]
        public int? SelectedProductId { get; set; } 

        [Display(Name = "Select Warehouses (Optional, multi-select)")]
        public List<int>? SelectedWarehouseIds { get; set; } 

        [Display(Name = "Start Date (Optional)")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [Display(Name = "End Date (Optional)")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

       
        public SelectList? AllProducts { get; set; }
        public SelectList? AllWarehouses { get; set; }

        
        public List<ProductInventoryReportViewModel>? ReportData { get; set; }
    }
}