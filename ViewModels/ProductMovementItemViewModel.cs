using System;
using Inventory_Management_System.Models; // Assuming models are in this namespace

namespace Inventory_Management_System.ViewModels
{
    public class ProductMovementItemViewModel
    {
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string WarehouseName { get; set; }
        public DateTime MovementDate { get; set; }
        public string MovementType { get; set; } // e.g., "Inbound", "Outbound", "Transfer (Out)", "Transfer (In)"
        public string ReferenceNumber { get; set; } // SupplyOrder.OrderNumber, ReleaseOrder.OrderNumber, ProductTransfer.TransferNumber
        public int Quantity { get; set; }
        public string RelatedParty { get; set; } // Supplier Name, Customer Name, or other Warehouse Name
        public DateTime? ProductionDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}