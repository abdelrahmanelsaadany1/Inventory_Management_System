using Inventory_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Inventory_Management_System.Controllers
{
    public class SupplierController : Controller
    {
        private readonly InventoryDbContext _context = new InventoryDbContext();

        public IActionResult Index()
        {
            var supplierList = _context.Suppliers.ToList();
            return View("Index", supplierList);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View("Add");
        }

        [HttpPost]
        public IActionResult SaveAdd(Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                // Check for duplicate supplier name
                if (_context.Suppliers.Any(s => s.Name.ToLower() == supplier.Name.ToLower()))
                {
                    ModelState.AddModelError("Name", "A supplier with this name already exists");
                    return View("Add", supplier);
                }

                // Check if at least one contact method is provided
                if (string.IsNullOrWhiteSpace(supplier.Phone) &&
                    string.IsNullOrWhiteSpace(supplier.Mobile) &&
                    string.IsNullOrWhiteSpace(supplier.Email))
                {
                    ModelState.AddModelError("", "At least one contact method (Phone, Mobile, or Email) is required");
                    return View("Add", supplier);
                }

                supplier.CreatedAt = DateTime.UtcNow;
                _context.Suppliers.Add(supplier);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Supplier added successfully!";
                return RedirectToAction("Index");
            }

            return View("Add", supplier);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var supplier = _context.Suppliers.Find(id);
            if (supplier == null)
            {
                TempData["ErrorMessage"] = "Supplier not found";
                return RedirectToAction("Index");
            }
            return View("Edit", supplier);
        }

        [HttpPost]
        public IActionResult SaveEdit(Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                var existing = _context.Suppliers.Find(supplier.Id);
                if (existing == null)
                {
                    TempData["ErrorMessage"] = "Supplier not found";
                    return RedirectToAction("Index");
                }

                // Check for duplicate supplier name (excluding current record)
                if (_context.Suppliers.Any(s => s.Name.ToLower() == supplier.Name.ToLower() && s.Id != supplier.Id))
                {
                    ModelState.AddModelError("Name", "A supplier with this name already exists");
                    return View("Edit", supplier);
                }

                // Check if at least one contact method is provided
                if (string.IsNullOrWhiteSpace(supplier.Phone) &&
                    string.IsNullOrWhiteSpace(supplier.Mobile) &&
                    string.IsNullOrWhiteSpace(supplier.Email))
                {
                    ModelState.AddModelError("", "At least one contact method (Phone, Mobile, or Email) is required");
                    return View("Edit", supplier);
                }

                // Update properties
                existing.Name = supplier.Name;
                existing.Phone = supplier.Phone;
                existing.Mobile = supplier.Mobile;
                existing.Email = supplier.Email;
                existing.Website = supplier.Website;
                existing.UpdatedAt = DateTime.UtcNow;

                _context.SaveChanges();

                TempData["SuccessMessage"] = "Supplier updated successfully!";
                return RedirectToAction("Index");
            }

            return View("Edit", supplier);
        }
    }
}