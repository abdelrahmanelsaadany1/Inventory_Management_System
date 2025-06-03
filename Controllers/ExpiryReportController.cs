// Controllers/ExpiryReportController.cs
using Inventory_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Inventory_Management_System.Controllers
{
    public class ExpiryReportController : Controller
    {
        private readonly InventoryDbContext _context = new InventoryDbContext();

        [HttpGet]
        public IActionResult Index(int daysThreshold = 90)
        {
            var allWarehouseProducts = _context.WarehouseProducts
                .Include(wp => wp.Product)
                .Include(wp => wp.Warehouse)
                .Include(wp => wp.Supplier)
                .ToList();

            var expiringProducts = allWarehouseProducts
                .Where(wp => !wp.IsExpired && wp.DaysUntilExpiry <= daysThreshold)
                .OrderBy(wp => wp.DaysUntilExpiry)
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
                .ToList();

            ViewBag.DaysThreshold = daysThreshold;
            return View("ExpiryReport", expiringProducts);
        }
    }
}
