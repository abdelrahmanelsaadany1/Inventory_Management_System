using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.Models
{
    public class Customer
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        [Required(ErrorMessage = "Supplier name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Supplier name must be between 2 and 100 characters")]
        public string Name { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        public string Phone { get; set; }

        [Phone(ErrorMessage = "Invalid mobile number format")]
        
        public string Mobile { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Url(ErrorMessage = "Invalid website URL format")]
        [StringLength(200, ErrorMessage = "Website URL cannot exceed 200 characters")]
        public string Website { get; set; }

        public ICollection<ReleaseOrder> ReleaseOrders { get; set; } = new HashSet<ReleaseOrder>();
    }
}