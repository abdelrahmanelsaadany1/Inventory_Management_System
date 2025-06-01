namespace Inventory_Management_System.Models
{
    public class SupplyOrderItem
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public int SupplyOrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime ProductionDate { get; set; }
        public int ExpiryPeriodInDays { get; set; }

        // Navigation Properties
        public SupplyOrder SupplyOrder { get; set; }
        public Product Product { get; set; }
    }
}
