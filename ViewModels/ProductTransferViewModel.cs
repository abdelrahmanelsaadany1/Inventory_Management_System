using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.ViewModels
{
    public class ProductTransferViewModel
    {
        [Required(ErrorMessage = "تاريخ التحويل مطلوب")]
        [Display(Name = "تاريخ التحويل")]
        public DateTime TransferDate { get; set; }

        [Required(ErrorMessage = "المخزن المصدر مطلوب")]
        [Display(Name = "المخزن المصدر")]
        public int SourceWarehouseId { get; set; }

        [Required(ErrorMessage = "المخزن المقصد مطلوب")]
        [Display(Name = "المخزن المقصد")]
        public int DestinationWarehouseId { get; set; }

        public List<ProductTransferItemViewModel> Items { get; set; } = new List<ProductTransferItemViewModel>();
    }

    public class ProductTransferItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        
        public DateTime ProductionDate { get; set; }
        public int ExpiryPeriodInDays { get; set; }
        
        [Required(ErrorMessage = "الكمية مطلوبة")]
        [Range(1, int.MaxValue, ErrorMessage = "الكمية يجب أن تكون أكبر من صفر")]
        public int Quantity { get; set; }
        
        public int AvailableQuantity { get; set; }
    }
}
