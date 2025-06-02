// Models/StaleProductsReportViewModel.cs
using System;

namespace Inventory_Management_System.Models
{
    public class StaleProductsReportViewModel
    {
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string WarehouseName { get; set; }
        public string SupplierName { get; set; }
        public int Quantity { get; set; }
        public DateTime ProductionDate { get; set; }
        public DateTime EntryDateIntoWarehouse { get; set; } // Using CreatedAt from WarehouseProduct
        public int DaysInWarehouse { get; set; } // Calculated based on EntryDateIntoWarehouse
    }
}