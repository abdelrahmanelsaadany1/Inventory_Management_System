using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory_Management_System.Models
{
    public class WarehouseProduct
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }

        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }

        public int Quantity { get; set; }
        public DateTime ProductionDate { get; set; }

        public int ExpiryPeriodInDays { get; set; }

        [NotMapped]
        public DateTime ExpiryDate
        {
            get { return ProductionDate.AddDays(ExpiryPeriodInDays); }
        }

        [NotMapped]
        public int DaysUntilExpiry
        {
            get
            {
                TimeSpan timeRemaining = ExpiryDate.Date - DateTime.Today.Date;
                return (int)Math.Ceiling(timeRemaining.TotalDays);
            }
        }

        [NotMapped]
        public bool IsExpired
        {
            get { return DateTime.Today.Date > ExpiryDate.Date; }
        }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}