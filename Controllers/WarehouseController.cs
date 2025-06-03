using Inventory_Management_System.Models;
// Assuming your DbContext is in Inventory_Management_System.Data. 
// If it's in the same Models namespace, you might not need this.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // Required for SelectList
using Microsoft.EntityFrameworkCore; // Required for .Include() and .ToList()

namespace Inventory_Management_System.Controllers
{
    public class WarehouseController : Controller
    {
        // Keeping your direct DbContext instantiation as requested.
        // WARNING: This is NOT the recommended way to use DbContext in ASP.NET Core
        // and can lead to issues in production environments.
        private readonly InventoryDbContext _context = new InventoryDbContext();

        // List all warehouses
        public IActionResult Index()
        {
            // Eagerly load the Manager for each Warehouse when listing them.
            // This prevents N+1 query issues if you display Manager.Name in the view.
            var warehouseList = _context.Warehouses
                                        .Include(w => w.Manager) // Include the Manager navigation property
                                        .ToList();
            return View("Index", warehouseList);
        }

        // Show add warehouse form
        [HttpGet]
        public IActionResult Add()
        {
            // Populate ViewBag with available Managers for the dropdown
            PopulateManagersDropDownList();
            return View("Add");
        }

        // Handle add warehouse form POST
        [HttpPost]
        // [ValidateAntiForgeryToken] // Strongly recommended for POST actions for security
        public IActionResult SaveAdd(Warehouse warehouse)
        {
            // Using basic validation as per your original code.
            // Consider using ModelState.IsValid for more robust validation.
            if (!string.IsNullOrWhiteSpace(warehouse.Name))
            {
                warehouse.CreatedAt = DateTime.UtcNow;
                warehouse.UpdatedAt = DateTime.UtcNow; // It's good practice to set UpdatedAt on creation too

                // The ManagerId will be bound directly from the form submission to the warehouse object
                _context.Warehouses.Add(warehouse);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            // If validation fails, re-populate managers before returning the view
            PopulateManagersDropDownList();
            return View("Add", warehouse);
        }

        // Show edit warehouse form
        [HttpGet]
        public IActionResult Edit(int id)
        {
            // Find the warehouse and eager load its Manager
            var warehouse = _context.Warehouses
                                    .Include(w => w.Manager) // Include Manager for display/pre-selection
                                    .FirstOrDefault(w => w.Id == id); // Use FirstOrDefault for safety

            if (warehouse == null) return NotFound();

            // Populate ViewBag with managers, pre-selecting the current manager of the warehouse
            PopulateManagersDropDownList(warehouse.ManagerId);
            return View("Edit", warehouse);
        }

        // Handle edit warehouse form POST
        [HttpPost]
        // [ValidateAntiForgeryToken] // Strongly recommended for POST actions for security
        public IActionResult SaveEdit(Warehouse warehouse)
        {
            // Using basic validation as per your original code.
            // Consider using ModelState.IsValid for more robust validation.
            if (!string.IsNullOrWhiteSpace(warehouse.Name))
            {
                var existing = _context.Warehouses.Find(warehouse.Id);
                if (existing == null) return NotFound();

                existing.Name = warehouse.Name;
                existing.Address = warehouse.Address;
                existing.UpdatedAt = DateTime.UtcNow;

                // IMPORTANT: Update the ManagerId from the submitted warehouse model
                existing.ManagerId = warehouse.ManagerId;

                _context.Warehouses.Update(existing); // Or _context.Entry(existing).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            // If validation fails, re-populate managers before returning the view
            PopulateManagersDropDownList(warehouse.ManagerId);
            return View("Edit", warehouse);
        }

        // Helper method to populate the dropdown list of managers
        private void PopulateManagersDropDownList(object selectedManager = null)
        {
            // Fetch all managers, ordered by name
            var managersQuery = _context.Managers.OrderBy(m => m.Name);

            // Create a SelectList and store it in ViewBag for the view to use
            ViewBag.Managers = new SelectList(managersQuery.AsNoTracking(), "Id", "Name", selectedManager);
        }
    }
}