// Models/ProductInventoryReportViewModel.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.Models
{
    public class ProductInventoryReportViewModel
    {
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string WarehouseName { get; set; }
        public string SupplierName { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")] 
        public int Quantity { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime ProductionDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime EntryDateIntoWarehouse { get; set; } 
    }
}