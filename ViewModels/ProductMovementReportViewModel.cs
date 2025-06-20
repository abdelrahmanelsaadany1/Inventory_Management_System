using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.ViewModels
{
    public class ProductMovementReportViewModel
    {
        [Display(Name = "From Date")]
        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }

        [Display(Name = "To Date")]
        [DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }

        [Display(Name = "Product")]
        public int? ProductId { get; set; }

        [Display(Name = "Warehouse")]
        public int? WarehouseId { get; set; }

        public List<ProductMovementItem> Movements { get; set; }

        public ProductMovementReportViewModel()
        {
            Movements = new List<ProductMovementItem>();
        }
    }

    public class ProductMovementItem
    {
        public DateTime Date { get; set; }
        public string TransferNumber { get; set; }
        public string ProductName { get; set; }
        public string SupplierName { get; set; }
        public string SourceWarehouse { get; set; }
        public string DestinationWarehouse { get; set; }
        public int Quantity { get; set; }
        public DateTime ProductionDate { get; set; }
        public int ExpiryPeriodInDays { get; set; }
    }
}