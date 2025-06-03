// Controllers/StaleProductsReportController.cs
using Inventory_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic; 

namespace Inventory_Management_System.Controllers
{
    public class StaleProductsReportController : Controller
    {
        private readonly InventoryDbContext _context = new InventoryDbContext();

        [HttpGet]
        public IActionResult Index() 
        {
            var allWarehouseProducts = _context.WarehouseProducts
                .Include(wp => wp.Product)    
                .Include(wp => wp.Warehouse)  
                .Include(wp => wp.Supplier)   
                .ToList(); 

           
            var reportProducts = allWarehouseProducts
              
                .OrderByDescending(wp => wp.UpdatedAt ?? wp.CreatedAt)
                .Select(wp => new StaleProductsReportViewModel
                {
                    ProductName = wp.Product.Name,
                    ProductCode = wp.Product.Code,
                    WarehouseName = wp.Warehouse.Name,
                    SupplierName = wp.Supplier.Name,
                    Quantity = wp.Quantity,
                    ProductionDate = wp.ProductionDate,
                    EntryDateIntoWarehouse = wp.CreatedAt, 
                   
                    DaysInWarehouse = (int)(DateTime.Today - (wp.UpdatedAt ?? wp.CreatedAt).Date).TotalDays
                })
                .ToList();

           
            return View("StaleProductsReport", reportProducts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}