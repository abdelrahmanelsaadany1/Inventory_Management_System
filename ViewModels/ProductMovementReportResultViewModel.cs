using System.Collections.Generic;

namespace Inventory_Management_System.ViewModels
{
    public class ProductMovementReportResultViewModel
    {
        public ReportMovementViewModel ReportParameters { get; set; }
        public List<ProductMovementItemViewModel> MovementItems { get; set; } = new List<ProductMovementItemViewModel>();
        public Dictionary<string, decimal> ProductSummary { get; set; } = new Dictionary<string, decimal>(); // Optional: Summary for each product
    }
}