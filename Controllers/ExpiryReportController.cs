// Controllers/ExpiryReportController.cs
using Inventory_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Required for .Include() and ToList()
using System;
using System.Linq;

namespace Inventory_Management_System.Controllers
{
    public class ExpiryReportController : Controller
    {
        private readonly InventoryDbContext _context = new InventoryDbContext();

        [HttpGet]
        public IActionResult Index(int daysThreshold = 90) // Default to 90 days, can be passed as a query parameter
        {
            var allWarehouseProducts = _context.WarehouseProducts
                .Include(wp => wp.Product)    // Eager load Product details
                .Include(wp => wp.Warehouse)   // Eager load Warehouse details
                .Include(wp => wp.Supplier)    // Eager load Supplier details
                .ToList(); // <--- IMPORTANT: This executes the query against the database and brings data into memory.

            var expiringProducts = allWarehouseProducts
                .Where(wp => !wp.IsExpired && wp.DaysUntilExpiry <= daysThreshold) // This filtering is now done in memory.
                .OrderBy(wp => wp.DaysUntilExpiry) // Ordering can also be done in memory.
                .Select(wp => new ExpiryReportViewModel
                {
                    ProductName = wp.Product.Name,
                    ProductCode = wp.Product.Code,
                    WarehouseName = wp.Warehouse.Name,
                    SupplierName = wp.Supplier.Name,
                    Quantity = wp.Quantity,
                    ProductionDate = wp.ProductionDate,
                    ExpiryPeriodInDays = wp.ExpiryPeriodInDays,
                    ExpiryDate = wp.ExpiryDate,
                    DaysUntilExpiry = wp.DaysUntilExpiry
                })
                .ToList(); // Final ToList to materialize the ViewModel collection.

            ViewBag.DaysThreshold = daysThreshold; // Pass the threshold to the view
            return View("ExpiryReport", expiringProducts);
        }
    }
}