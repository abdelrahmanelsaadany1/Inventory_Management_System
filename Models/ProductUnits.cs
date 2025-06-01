namespace Inventory_Management_System.Models
{
    public class ProductUnits
    {
        public int ProductId { get; set; } 
        public int UnitId { get; set; }     

        public Product Product { get; set; }
        public Unit Unit { get; set; }
    }
}
