namespace Inventory_Management_System.Models
{
    public class SupplyOrder
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public int WarehouseId { get; set; }
        public int SupplierId { get; set; }

        // Navigation Properties
        public Warehouse Warehouse { get; set; }
        public Supplier Supplier { get; set; }
        public ICollection<SupplyOrderItem> Items { get; set; } = new HashSet<SupplyOrderItem>();
    }
}
