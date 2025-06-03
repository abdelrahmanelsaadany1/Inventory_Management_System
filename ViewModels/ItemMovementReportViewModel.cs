using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.ViewModels
{
    public class ItemMovementReportViewModel
    {
        [Required]
        [Display(Name = "تاريخ البداية")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "تاريخ النهاية")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Display(Name = "المخازن")]
        public List<int> SelectedWarehouseIds { get; set; }

        public List<WarehouseDto> AvailableWarehouses { get; set; }

        public List<ItemMovementResult> Results { get; set; }

        public ItemMovementReportViewModel()
        {
            SelectedWarehouseIds = new List<int>();
            AvailableWarehouses = new List<WarehouseDto>();
            Results = new List<ItemMovementResult>();
        }
    }

    public class WarehouseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ItemMovementResult
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string WarehouseName { get; set; }
        public int InitialQuantity { get; set; }
        public int IncomingQuantity { get; set; }
        public int OutgoingQuantity { get; set; }
        public int FinalQuantity { get; set; }
    }
} 