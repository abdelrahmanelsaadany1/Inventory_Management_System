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

       
        public Warehouse SourceWarehouse { get; set; }
        public Warehouse DestinationWarehouse { get; set; }
       
        [JsonIgnore] 
        public ICollection<ProductTransferItem> Items { get; set; } = new HashSet<ProductTransferItem>();
    }
}
