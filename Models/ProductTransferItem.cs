using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Inventory_Management_System.Models
{
    public class ProductTransferItem
    {
        public int Id { get; set; } // PK for the item
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public int ProductTransferId { get; set; }
        [JsonIgnore]
        public ProductTransfer ProductTransfer { get; set; }

        [Required(ErrorMessage = "Product is required.")]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Supplier is required for the product batch.")]
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }

        [Required(ErrorMessage = "Production date is required for the product batch.")]
        [DataType(DataType.Date)]
        public DateTime ProductionDate { get; set; }

        [Required(ErrorMessage = "Expiry period is required for the product batch.")]
        [Range(1, int.MaxValue, ErrorMessage = "Expiry period must be greater than 0.")]
        public int ExpiryPeriodInDays { get; set; }
    }
}
