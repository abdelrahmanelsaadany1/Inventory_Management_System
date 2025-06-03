using Inventory_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Inventory_Management_System.Controllers
{
    public class CustomerController : Controller
    {
        private readonly InventoryDbContext _context = new InventoryDbContext();

     
        public IActionResult Index()
        {
            var customers = _context.Customers.ToList();
            return View(customers);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SaveAdd(Customer customer)
        {
            if (ModelState.IsValid)
            {
                customer.CreatedAt = DateTime.UtcNow;
                _context.Customers.Add(customer);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("Add", customer);
        }

      
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var customer = _context.Customers.Find(id);
            if (customer == null) return NotFound();
            return View(customer);
        }

      
        [HttpPost]
        public IActionResult SaveEdit(Customer customer)
        {
            if (ModelState.IsValid)
            {
                var existing = _context.Customers.Find(customer.Id);
                if (existing == null) return NotFound();

                existing.Name = customer.Name;
                existing.Phone = customer.Phone;
                existing.Fax = customer.Fax;
                existing.Mobile = customer.Mobile;
                existing.Email = customer.Email;
                existing.Website = customer.Website;
                existing.UpdatedAt = DateTime.UtcNow;

                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("Edit", customer);
        }
    }
}
