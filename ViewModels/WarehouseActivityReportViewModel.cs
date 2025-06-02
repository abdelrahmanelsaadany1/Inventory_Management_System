// ViewModels/WarehouseActivityReportViewModel.cs
// (Create this new file in your ViewModels folder)
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
        public int QuantityInWarehouse { get; set; } // Quantity of this product batch currently in the selected warehouse

        [Display(Name = "Production Date")]
        [DataType(DataType.Date)]
        public DateTime ProductionDate { get; set; }

        [Display(Name = "Entry Date into Warehouse")]
        [DataType(DataType.Date)]
        public DateTime EntryDateIntoWarehouse { get; set; } // Corresponds to WarehouseProduct.CreatedAt

        [Display(Name = "Days In Warehouse")]
        public int DaysInWarehouse { get; set; } // How long this batch has been there
    }
}