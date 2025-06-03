using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Inventory_Management_System.Models
{
    public class Product
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public string Code { get; set; } // Unique
        public string Name { get; set; }
        [JsonIgnore]
        public ICollection<ProductUnits> ProductUnits { get; set; } = new HashSet<ProductUnits>();
        [JsonIgnore]
        public ICollection<WarehouseProduct> WarehouseProducts { get; set; } = new HashSet<WarehouseProduct>();
        [JsonIgnore]
        public ICollection<SupplyOrderItem> SupplyOrderItems { get; set; } = new HashSet<SupplyOrderItem>();
        [JsonIgnore]
        public ICollection<ReleaseOrderItem> ReleaseOrderItems { get; set; } = new HashSet<ReleaseOrderItem>();
        [JsonIgnore]
        public ICollection<ProductTransfer> ProductTransfers { get; set; } = new HashSet<ProductTransfer>();

        [Display(Name = "Default Expiry Period (Days)")]
        [Range(0, int.MaxValue, ErrorMessage = "Expiry period must be 0 or greater.")]
        public int ExpiryPeriodInDays { get; set; }

    }
}
