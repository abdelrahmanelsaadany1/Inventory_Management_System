// Inventory_Management_System.Controllers/ManagerController.cs

using Inventory_Management_System.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // For .ToList(), .FirstOrDefault(), etc.
using System.Linq; // For OrderBy

namespace Inventory_Management_System.Controllers
{
    public class ManagerController : Controller
    {
        // Keeping your direct DbContext instantiation for consistency with your WarehouseController.
        // REMINDER: For production, use Dependency Injection.
        private readonly InventoryDbContext _context = new InventoryDbContext();

        // GET: /Manager/Index
        // Lists all managers
        public IActionResult Index()
        {
            var managers = _context.Managers.ToList();
            return View("Index", managers);
        }

        // GET: /Manager/Add
        // Shows the form to add a new manager
        [HttpGet]
        public IActionResult Add()
        {
            return View("Add");
        }

        // POST: /Manager/SaveAdd
        // Handles the form submission for adding a new manager
        [HttpPost]
        [ValidateAntiForgeryToken] // Always use for POST actions
        public IActionResult SaveAdd(Manager manager)
        {
            // Basic validation: Check if the manager name is provided
            if (string.IsNullOrWhiteSpace(manager.Name))
            {
                ModelState.AddModelError("Name", "Manager Name is required.");
            }

            if (ModelState.IsValid) // Check if model validation passes
            {
                manager.CreatedAt = DateTime.UtcNow;
                manager.UpdatedAt = DateTime.UtcNow; // Set on creation too
                _context.Managers.Add(manager);
                _context.SaveChanges();
                return RedirectToAction("Index"); // Redirect to the list of managers
            }

            // If validation fails, return to the Add view with the current model
            return View("Add", manager);
        }

        // GET: /Manager/Edit/{id}
        // Shows the form to edit an existing manager
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var manager = _context.Managers.Find(id);
            if (manager == null)
            {
                return NotFound();
            }
            return View("Edit", manager);
        }

        // POST: /Manager/SaveEdit
        // Handles the form submission for editing a manager
        [HttpPost]
        [ValidateAntiForgeryToken] // Always use for POST actions
        public IActionResult SaveEdit(Manager manager)
        {
            // Basic validation: Check if the manager name is provided
            if (string.IsNullOrWhiteSpace(manager.Name))
            {
                ModelState.AddModelError("Name", "Manager Name is required.");
            }

            if (ModelState.IsValid) // Check if model validation passes
            {
                var existingManager = _context.Managers.Find(manager.Id);
                if (existingManager == null)
                {
                    return NotFound();
                }

                existingManager.Name = manager.Name;
                existingManager.ContactInfo = manager.ContactInfo; // Update contact info
                existingManager.UpdatedAt = DateTime.UtcNow;

                _context.Managers.Update(existingManager); // Or _context.Entry(existingManager).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("Index"); // Redirect to the list of managers
            }

            // If validation fails, return to the Edit view with the current model
            return View("Edit", manager);
        }

        // GET: /Manager/Delete/{id} (Optional: Confirmation page for delete)
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var manager = _context.Managers.Find(id);
            if (manager == null)
            {
                return NotFound();
            }
            return View("Delete", manager); // Assuming you'll create a Delete.cshtml for confirmation
        }

        // POST: /Manager/ConfirmDelete/{id}
        // Handles the actual deletion
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmDelete(int id)
        {
            var manager = _context.Managers.Find(id);
            if (manager == null)
            {
                return NotFound();
            }

            _context.Managers.Remove(manager);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}