using Inventory_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Management_System.Controllers
{
    public class WarehouseController : Controller
    {
        private readonly InventoryDbContext _context = new InventoryDbContext();

        // List all warehouses
        public IActionResult Index()
        {
            var warehouseList = _context.Warehouses.ToList();
            return View("Index", warehouseList);
        }

        // Show add warehouse form
        [HttpGet]
        public IActionResult Add()
        {
            return View("Add");
        }

        // Handle add warehouse form POST
        [HttpPost]
        public IActionResult SaveAdd(Warehouse warehouse)
        {
            if (!string.IsNullOrWhiteSpace(warehouse.Name))
            {
                warehouse.CreatedAt = DateTime.UtcNow;
                _context.Warehouses.Add(warehouse);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("Add", warehouse);
        }

        // Show edit warehouse form
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var warehouse = _context.Warehouses.Find(id);
            if (warehouse == null) return NotFound();
            return View("Edit", warehouse);
        }

        // Handle edit warehouse form POST
        [HttpPost]
        public IActionResult SaveEdit(Warehouse warehouse)
        {
            if (!string.IsNullOrWhiteSpace(warehouse.Name))
            {
                var existing = _context.Warehouses.Find(warehouse.Id);
                if (existing == null) return NotFound();

                existing.Name = warehouse.Name;
                existing.Address = warehouse.Address;
                existing.UpdatedAt = DateTime.UtcNow;

                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("Edit", warehouse);
        }
    }
}
