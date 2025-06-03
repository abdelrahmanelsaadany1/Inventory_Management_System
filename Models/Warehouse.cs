using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory_Management_System.Models
{
    public class Warehouse
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public string Name { get; set; }
        public string Address { get; set; }
        public int? ManagerId { get; set; }
        [ForeignKey("ManagerId")] // Specifies that ManagerId is the foreign key
        public Manager Manager { get; set; }
        public ICollection<WarehouseProduct> WarehouseProducts { get; set; } = new HashSet<WarehouseProduct>();
        public ICollection<SupplyOrder> SupplyOrders { get; set; } = new HashSet<SupplyOrder>();
        public ICollection<ReleaseOrder> ReleaseOrders { get; set; } = new HashSet<ReleaseOrder>();
        public ICollection<ProductTransfer> SourceTransfers { get; set; } = new HashSet<ProductTransfer>();
        public ICollection<ProductTransfer> DestinationTransfers { get; set; } = new HashSet<ProductTransfer>();


    }
}
