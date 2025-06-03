using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory_Management_System.Models
{
    public class InventoryTransaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ItemId { get; set; }

        [Required]
        public int WarehouseId { get; set; }

        [Required]
        public TransactionType Type { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public string Reference { get; set; }

        public string Notes { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("ItemId")]
        public virtual Item Item { get; set; }

        [ForeignKey("WarehouseId")]
        public virtual Warehouse Warehouse { get; set; }
    }
} 