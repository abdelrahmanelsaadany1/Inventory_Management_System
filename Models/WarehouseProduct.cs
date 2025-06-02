// Models/WarehouseProduct.cs
using System;
using System.ComponentModel.DataAnnotations.Schema; // Add this for [NotMapped] if needed for clarity

namespace Inventory_Management_System.Models
{
    public class WarehouseProduct
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; } // Navigation property

        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; } // Navigation property

        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; } // Navigation property

        public int Quantity { get; set; }
        public DateTime ProductionDate { get; set; }
  
        public int ExpiryPeriodInDays { get; set; } // Represents the number of days for expiry

        // Calculated property: ExpiryDate
        // This calculates the expiry date based on ProductionDate and ExpiryPeriodInDays
        [NotMapped] // Inform EF Core not to map this to a database column
        public DateTime ExpiryDate
        {
            get { return ProductionDate.AddDays(ExpiryPeriodInDays); }
        }

        // Calculated property: DaysUntilExpiry
        // This calculates the remaining days until expiry
        [NotMapped] // Inform EF Core not to map this to a database column
        public int DaysUntilExpiry
        {
            get
            {
                // Calculate difference in days. Use ToUniversalTime to ensure consistency
                // or ensure all DateTime operations in your application are timezone-aware.
                // For simplicity, using DateTime.Today for comparison.
                TimeSpan timeRemaining = ExpiryDate.Date - DateTime.Today.Date;
                return (int)Math.Ceiling(timeRemaining.TotalDays);
            }
        }

        // Calculated property: IsExpired
        // This determines if the product has expired
        [NotMapped] // Inform EF Core not to map this to a database column
        public bool IsExpired
        {
            get { return DateTime.Today.Date > ExpiryDate.Date; }
        }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; } // Nullable DateTime

        // You might have other properties here, e.g., BatchNumber, etc.
    }
}