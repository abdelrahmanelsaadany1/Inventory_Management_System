using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory_Management_System.Models
{
    public class WarehouseProduct
    {
       
        public int WarehouseId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime ProductionDate { get; set; }
        public int ExpiryPeriodInDays { get; set; }
        public int SupplierId { get; set; }

       
        public DateTime ExpiryDate => ProductionDate.AddDays(ExpiryPeriodInDays);
        public bool IsExpired => DateTime.Now > ExpiryDate;
        public int DaysUntilExpiry => (ExpiryDate - DateTime.Now).Days;
        public Warehouse Warehouse { get; set; }
        public Product Product { get; set; }
        public Supplier Supplier { get; set; }
    }
}
