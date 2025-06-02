// Controllers/StaleProductsReportController.cs
using Inventory_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic; // Ensure this is present

namespace Inventory_Management_System.Controllers
{
    public class StaleProductsReportController : Controller
    {
        private readonly InventoryDbContext _context = new InventoryDbContext();

        // GET: StaleProductsReport (Now displays all products with their days in warehouse)
        [HttpGet]
        public IActionResult Index() // No daysInWarehouseThreshold parameter needed for display
        {
            // Fetch all relevant WarehouseProducts with their related entities.
            // .ToList() is used here to bring the data into memory
            // before calculating 'DaysInWarehouse', which uses calculated properties.
            var allWarehouseProducts = _context.WarehouseProducts
                .Include(wp => wp.Product)    // Eager load Product details
                .Include(wp => wp.Warehouse)   // Eager load Warehouse details
                .Include(wp => wp.Supplier)    // Eager load Supplier details
                .ToList(); // Execute query and bring data to client memory

            // Project all fetched data to the StaleProductsReportViewModel
            var reportProducts = allWarehouseProducts
                // Order by most recently updated/created first, or by product name
                .OrderByDescending(wp => wp.UpdatedAt ?? wp.CreatedAt)
                .Select(wp => new StaleProductsReportViewModel
                {
                    ProductName = wp.Product.Name,
                    ProductCode = wp.Product.Code,
                    WarehouseName = wp.Warehouse.Name,
                    SupplierName = wp.Supplier.Name,
                    Quantity = wp.Quantity,
                    ProductionDate = wp.ProductionDate,
                    EntryDateIntoWarehouse = wp.CreatedAt, // Keep CreatedAt to show original entry date
                    // Calculate DaysInWarehouse for display based on UpdatedAt (or CreatedAt fallback)
                    DaysInWarehouse = (int)(DateTime.Today - (wp.UpdatedAt ?? wp.CreatedAt).Date).TotalDays
                })
                .ToList();

            // Removed ViewBag.DaysInWarehouseThreshold as it's no longer a filter
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