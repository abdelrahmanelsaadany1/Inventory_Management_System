

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; 

namespace Inventory_Management_System.Models
{
    public class Manager
    {
        public int Id { get; set; } 

        [Required] 
        [StringLength(100)] 
        public string Name { get; set; }

        [StringLength(255)] 
        public string ContactInfo { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 
        public DateTime? UpdatedAt { get; set; }

        
    }
}