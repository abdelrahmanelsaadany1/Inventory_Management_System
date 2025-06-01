using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Inventory_Management_System.Models
{
    public class ProductTransfer
    {

        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        [Required(ErrorMessage = "Transfer number is required.")]
        [StringLength(50, ErrorMessage = "Transfer number cannot exceed 50 characters.")]
        public string TransferNumber { get; set; }

        [Required(ErrorMessage = "Transfer date is required.")]
        public DateTime TransferDate { get; set; }

        [Required(ErrorMessage = "Source warehouse is required.")]
        public int SourceWarehouseId { get; set; }

        [Required(ErrorMessage = "Destination warehouse is required.")]
        public int DestinationWarehouseId { get; set; }

        // THESE PROPERTIES ARE REMOVED FROM PRODUCTTRANSFER (HEADER)
        // public int ProductId { get; set; }
        // public int Quantity { get; set; }
        // public int SupplierId { get; set; }
        // public DateTime ProductionDate { get; set; }
        // public int ExpiryPeriodInDays { get; set; }

        // Navigation Properties for the main transfer header
        public Warehouse SourceWarehouse { get; set; }
        public Warehouse DestinationWarehouse { get; set; }
        // Also remove direct navigation properties like Product Product and Supplier Supplier from here
        // public Product Product { get; set; }
        // public Supplier Supplier { get; set; }


        // THIS IS THE CRUCIAL ADDITION FOR THE HEADER-DETAIL PATTERN
        [JsonIgnore] // Important to prevent serialization cycles
        public ICollection<ProductTransferItem> Items { get; set; } = new HashSet<ProductTransferItem>();
    }
}
