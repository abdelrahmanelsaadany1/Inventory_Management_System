// Controllers/ProductInventoryReportController.cs
using Inventory_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // For SelectList
using Microsoft.EntityFrameworkCore; // Required for .Include()
using System;
using System.Collections.Generic;
using System.Linq;

namespace Inventory_Management_System.Controllers
{
    public class ProductInventoryReportController : Controller
    {
        private readonly InventoryDbContext _context = new InventoryDbContext();

        // GET: ProductInventoryReport
        [HttpGet]
        public IActionResult Index()
        {
            var model = new ProductInventoryReportRequestViewModel
            {
                AllProducts = new SelectList(_context.Products.OrderBy(p => p.Name), "Id", "Name"),
                AllWarehouses = new SelectList(_context.Warehouses.OrderBy(w => w.Name), "Id", "Name")
            };
            return View(model);
        }

        // POST: ProductInventoryReport/GenerateReport
        [HttpPost]
        public IActionResult Index(ProductInventoryReportRequestViewModel request)
        {
            // Re-populate dropdowns for the view (important if validation fails or view is re-rendered)
            request.AllProducts = new SelectList(_context.Products.OrderBy(p => p.Name), "Id", "Name", request.SelectedProductId);
            request.AllWarehouses = new SelectList(_context.Warehouses.OrderBy(w => w.Name), "Id", "Name", request.SelectedWarehouseIds);

            IQueryable<WarehouseProduct> query = _context.WarehouseProducts
                .Include(wp => wp.Product)
                .Include(wp => wp.Warehouse)
                .Include(wp => wp.Supplier); // Assuming Supplier is linked to WarehouseProduct

            // Apply filters based on user input
            if (request.SelectedProductId.HasValue)
            {
                query = query.Where(wp => wp.ProductId == request.SelectedProductId.Value);
            }

            if (request.SelectedWarehouseIds != null && request.SelectedWarehouseIds.Any())
            {
                query = query.Where(wp => request.SelectedWarehouseIds.Contains(wp.WarehouseId));
            }

            if (request.StartDate.HasValue)
            {
                // Ensure comparison is only on the date part
                query = query.Where(wp => wp.CreatedAt.Date >= request.StartDate.Value.Date);
            }

            if (request.EndDate.HasValue)
            {
                // Ensure comparison is only on the date part, and include the end date's entire day
                query = query.Where(wp => wp.CreatedAt.Date <= request.EndDate.Value.Date);
            }

            // Order for consistency
            query = query.OrderBy(wp => wp.Product.Name).ThenBy(wp => wp.Warehouse.Name).ThenBy(wp => wp.CreatedAt);

            // Project to the ReportData ViewModel
            request.ReportData = query.Select(wp => new ProductInventoryReportViewModel
            {
                ProductName = wp.Product.Name,
                ProductCode = wp.Product.Code,
                WarehouseName = wp.Warehouse.Name,
                SupplierName = wp.Supplier.Name,
                Quantity = wp.Quantity,
                ProductionDate = wp.ProductionDate,
                EntryDateIntoWarehouse = wp.CreatedAt
            }).ToList(); // Execute the query and bring data into memory

            return View(request); // Return the same view, but now with the report data populated
        }
    }
}