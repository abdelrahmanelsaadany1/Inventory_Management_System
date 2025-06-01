namespace Inventory_Management_System.Models
{
    public class Unit
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public ICollection<ProductUnits> ProductUnits { get; set; } = new HashSet<ProductUnits>();
    }
}
