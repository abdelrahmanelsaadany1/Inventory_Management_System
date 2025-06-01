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

        [Required(ErrorMessage = "Customer name is required")]
        public string Name { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        public string Phone { get; set; }

        [Phone(ErrorMessage = "Invalid fax number")]
        public string Fax { get; set; }

        [Phone(ErrorMessage = "Invalid mobile number")]
        public string Mobile { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Url(ErrorMessage = "Invalid website URL")]
        public string Website { get; set; }

        public ICollection<ReleaseOrder> ReleaseOrders { get; set; } = new HashSet<ReleaseOrder>();
    }
}
