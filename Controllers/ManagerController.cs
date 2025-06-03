// Inventory_Management_System.Controllers/ManagerController.cs

using Inventory_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Inventory_Management_System.Controllers
{
    public class ManagerController : Controller
    {
        private readonly InventoryDbContext _context = new InventoryDbContext();

        public IActionResult Index()
        {
            var managers = _context.Managers.ToList();
            return View("Index", managers);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View("Add");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveAdd(Manager manager)
        {
            if (string.IsNullOrWhiteSpace(manager.Name))
            {
                ModelState.AddModelError("Name", "Manager Name is required.");
            }

            if (ModelState.IsValid)
            {
                manager.CreatedAt = DateTime.UtcNow;
                manager.UpdatedAt = DateTime.UtcNow;
                _context.Managers.Add(manager);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View("Add", manager);
        }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveEdit(Manager manager)
        {
            if (string.IsNullOrWhiteSpace(manager.Name))
            {
                ModelState.AddModelError("Name", "Manager Name is required.");
            }

            if (ModelState.IsValid)
            {
                var existingManager = _context.Managers.Find(manager.Id);
                if (existingManager == null)
                {
                    return NotFound();
                }

                existingManager.Name = manager.Name;
                existingManager.ContactInfo = manager.ContactInfo;
                existingManager.UpdatedAt = DateTime.UtcNow;

                _context.Managers.Update(existingManager);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View("Edit", manager);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var manager = _context.Managers.Find(id);
            if (manager == null)
            {
                return NotFound();
            }
            return View("Delete", manager);
        }

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
