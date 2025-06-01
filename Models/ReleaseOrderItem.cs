namespace Inventory_Management_System.Models
{
    public class ReleaseOrderItem
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public int ReleaseOrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        // Navigation Properties
        public ReleaseOrder ReleaseOrder { get; set; }
        public Product Product { get; set; }
    }
}
