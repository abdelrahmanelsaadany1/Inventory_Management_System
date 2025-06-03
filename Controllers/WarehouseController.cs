using Inventory_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Management_System.Controllers
{
    public class WarehouseController : Controller
    {
        private readonly InventoryDbContext _context = new InventoryDbContext();

        public IActionResult Index()
        {
            var warehouseList = _context.Warehouses
                                        .Include(w => w.Manager)
                                        .ToList();
            return View("Index", warehouseList);
        }

        [HttpGet]
        public IActionResult Add()
        {
            PopulateManagersDropDownList();
            return View("Add");
        }

        [HttpPost]
        public IActionResult SaveAdd(Warehouse warehouse)
        {
            if (!string.IsNullOrWhiteSpace(warehouse.Name))
            {
                warehouse.CreatedAt = DateTime.UtcNow;
                warehouse.UpdatedAt = DateTime.UtcNow;

                _context.Warehouses.Add(warehouse);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            PopulateManagersDropDownList();
            return View("Add", warehouse);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var warehouse = _context.Warehouses
                                    .Include(w => w.Manager)
                                    .FirstOrDefault(w => w.Id == id);

            if (warehouse == null) return NotFound();

            PopulateManagersDropDownList(warehouse.ManagerId);
            return View("Edit", warehouse);
        }

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

                existing.ManagerId = warehouse.ManagerId;

                _context.Warehouses.Update(existing);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            PopulateManagersDropDownList(warehouse.ManagerId);
            return View("Edit", warehouse);
        }

        private void PopulateManagersDropDownList(object selectedManager = null)
        {
            var managersQuery = _context.Managers.OrderBy(m => m.Name);

            ViewBag.Managers = new SelectList(managersQuery.AsNoTracking(), "Id", "Name", selectedManager);
        }
    }
}