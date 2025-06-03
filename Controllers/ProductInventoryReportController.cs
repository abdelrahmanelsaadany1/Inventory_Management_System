// Controllers/ProductInventoryReportController.cs
using Inventory_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Inventory_Management_System.Controllers
{
    public class ProductInventoryReportController : Controller
    {
        private readonly InventoryDbContext _context = new InventoryDbContext();

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

        [HttpPost]
        public IActionResult Index(ProductInventoryReportRequestViewModel request)
        {
            request.AllProducts = new SelectList(_context.Products.OrderBy(p => p.Name), "Id", "Name", request.SelectedProductId);
            request.AllWarehouses = new SelectList(_context.Warehouses.OrderBy(w => w.Name), "Id", "Name", request.SelectedWarehouseIds);

            IQueryable<WarehouseProduct> query = _context.WarehouseProducts
                .Include(wp => wp.Product)
                .Include(wp => wp.Warehouse)
                .Include(wp => wp.Supplier);

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
                query = query.Where(wp => wp.CreatedAt.Date >= request.StartDate.Value.Date);
            }

            if (request.EndDate.HasValue)
            {
                query = query.Where(wp => wp.CreatedAt.Date <= request.EndDate.Value.Date);
            }

            query = query.OrderBy(wp => wp.Product.Name).ThenBy(wp => wp.Warehouse.Name).ThenBy(wp => wp.CreatedAt);

            request.ReportData = query.Select(wp => new ProductInventoryReportViewModel
            {
                ProductName = wp.Product.Name,
                ProductCode = wp.Product.Code,
                WarehouseName = wp.Warehouse.Name,
                SupplierName = wp.Supplier.Name,
                Quantity = wp.Quantity,
                ProductionDate = wp.ProductionDate,
                EntryDateIntoWarehouse = wp.CreatedAt
            }).ToList();

            return View(request);
        }
    }
}
