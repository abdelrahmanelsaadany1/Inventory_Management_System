// ViewModels/WarehouseActivityReportViewModel.cs

using System;
using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.ViewModels
{
    public class WarehouseActivityReportViewModel
    {
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        [Display(Name = "Product Code")]
        public string ProductCode { get; set; }

        [Display(Name = "Supplier")]
        public string SupplierName { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int QuantityInWarehouse { get; set; } 

        [Display(Name = "Production Date")]
        [DataType(DataType.Date)]
        public DateTime ProductionDate { get; set; }

        [Display(Name = "Entry Date into Warehouse")]
        [DataType(DataType.Date)]
        public DateTime EntryDateIntoWarehouse { get; set; } 

        [Display(Name = "Days In Warehouse")]
        public int DaysInWarehouse { get; set; } 
    }
}