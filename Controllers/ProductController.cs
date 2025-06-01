using Inventory_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Inventory_Management_System.Controllers
{
    public class ProductController : Controller
    {
        private readonly InventoryDbContext _context = new InventoryDbContext();

    
        public IActionResult Index()
        {
            var productList = _context.Products.ToList();
            return View("Index", productList);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View("Add");
        }

       
        [HttpPost]
        public IActionResult SaveAdd(Product product)
        {
            if (!string.IsNullOrWhiteSpace(product.Name) && !string.IsNullOrWhiteSpace(product.Code))
            {
                product.CreatedAt = DateTime.UtcNow;
                _context.Products.Add(product);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("Add", product);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();
            return View("Edit", product);
        }

        
        [HttpPost]
        public IActionResult SaveEdit(Product product)
        {
            if (!string.IsNullOrWhiteSpace(product.Name) && !string.IsNullOrWhiteSpace(product.Code))
            {
                var existing = _context.Products.Find(product.Id);
                if (existing == null) return NotFound();

                existing.Name = product.Name;
                existing.Code = product.Code;
                existing.UpdatedAt = DateTime.UtcNow;

                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("Edit", product);
        }
    }
}
