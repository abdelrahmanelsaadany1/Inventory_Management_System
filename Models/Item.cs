using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.Models
{
    public class Item
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public string Unit { get; set; }

        public decimal MinimumQuantity { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public virtual ICollection<InventoryTransaction> InventoryTransactions { get; set; }
    }
} 