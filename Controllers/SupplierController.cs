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
            if (!string.IsNullOrWhiteSpace(supplier.Name))
            {
                supplier.CreatedAt = DateTime.UtcNow;
                _context.Suppliers.Add(supplier);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View("Add", supplier);
        }

     
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var supplier = _context.Suppliers.Find(id);
            if (supplier == null) return NotFound();
            return View("Edit", supplier);
        }

        
        [HttpPost]
        public IActionResult SaveEdit(Supplier supplier)
        {
            if (!string.IsNullOrWhiteSpace(supplier.Name))
            {
                var existing = _context.Suppliers.Find(supplier.Id);
                if (existing == null) return NotFound();

                existing.Name = supplier.Name;
                existing.Phone = supplier.Phone;
                existing.Fax = supplier.Fax;
                existing.Mobile = supplier.Mobile;
                existing.Email = supplier.Email;
                existing.Website = supplier.Website;
                existing.UpdatedAt = DateTime.UtcNow;

                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View("Edit", supplier);
        }
    }
}
