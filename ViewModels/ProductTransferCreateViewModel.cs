using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.ViewModels
{
    public class ProductTransferCreateViewModel
    {

        public string TransferNumber { get; set; }

        // Removed all validation attributes for debugging
        public DateTime TransferDate { get; set; } = DateTime.Now;

        // Removed all validation attributes for debugging
        public int SourceWarehouseId { get; set; }

        // Removed all validation attributes for debugging
        public int DestinationWarehouseId { get; set; }

        // These should NOT have [Required] anyway, as they are for dropdown population
        public SelectList AllWarehouses { get; set; }
        public SelectList AllProducts { get; set; }
        public SelectList AllSuppliers { get; set; }

        public List<ProductTransferItemViewModel> Items { get; set; } = new List<ProductTransferItemViewModel>();
    }
}

