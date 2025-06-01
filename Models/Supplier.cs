using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.Models
{
    public class Supplier
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        [Required(ErrorMessage = "Supplier name is required")]
        public string Name { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        public string Phone { get; set; }

        [Phone(ErrorMessage = "Invalid fax number format make it like 1234567890")]
        public string Fax { get; set; }

        [Phone(ErrorMessage = "Invalid mobile number format")]
        public string Mobile { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Url(ErrorMessage = "use format like http://example.com")]
        public string Website { get; set; }

        public ICollection<SupplyOrder> SupplyOrders { get; set; } = new HashSet<SupplyOrder>();
        public ICollection<WarehouseProduct> WarehouseProducts { get; set; } = new HashSet<WarehouseProduct>();
        public ICollection<ProductTransfer> ProductTransfers { get; set; } = new HashSet<ProductTransfer>();
    }
}
