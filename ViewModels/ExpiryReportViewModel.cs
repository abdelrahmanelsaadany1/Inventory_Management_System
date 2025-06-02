// Models/ExpiryReportViewModel.cs
using System;

namespace Inventory_Management_System.Models
{
    public class ExpiryReportViewModel
    {
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string WarehouseName { get; set; }
        public string SupplierName { get; set; }
        public int Quantity { get; set; }
        public DateTime ProductionDate { get; set; }
        public int ExpiryPeriodInDays { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int DaysUntilExpiry { get; set; }
    }
}