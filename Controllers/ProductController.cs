using Inventory_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore; // Added for DbUpdateConcurrencyException and Any()

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
            // Initialize with a default ExpiryPeriodInDays if desired, e.g., 0
            return View("Add", new Product { ExpiryPeriodInDays = 0 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // IMPORTANT: Added for security
        public IActionResult SaveAdd(Product product)
        {
            // Set CreatedAt timestamp before validation
            product.CreatedAt = DateTime.UtcNow;

            // --- START: Uniqueness Check for Code ---
            if (_context.Products.Any(p => p.Code == product.Code))
            {
                ModelState.AddModelError("Code", "Product Code already exists. Please use a unique code.");
            }
            // --- END: Uniqueness Check for Code ---

            // Use ModelState.IsValid to leverage data annotations from Product.cs
            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                try
                {
                    _context.SaveChanges();
                    TempData["SuccessMessage"] = "Product added successfully!"; // Optional: for notifications
                    return RedirectToAction("Index");
                }
                catch (DbUpdateException ex)
                {
                    // Catch database exceptions specifically related to unique constraints if needed
                    // For now, the upfront check handles the most common case
                    ModelState.AddModelError("", "An error occurred while saving. Please try again. " + ex.Message);
                    return View("Add", product);
                }
            }

            // If ModelState is not valid (due to data annotations or uniqueness check),
            // return to the Add view with the product model to display errors.
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
        [ValidateAntiForgeryToken] // IMPORTANT: Added for security
        public IActionResult SaveEdit(Product product)
        {
            // First, retrieve the existing product from the database
            var existingProduct = _context.Products.Find(product.Id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            // --- START: Uniqueness Check for Code in Edit ---
            // Check if the new Code is a duplicate AND belongs to a DIFFERENT product
            if (_context.Products.Any(p => p.Code == product.Code && p.Id != product.Id))
            {
                ModelState.AddModelError("Code", "Product Code already exists for another product. Please use a unique code.");
            }
            // --- END: Uniqueness Check for Code in Edit ---

            // Manually update properties from the submitted 'product' to 'existingProduct'
            existingProduct.Name = product.Name;
            existingProduct.Code = product.Code;
            existingProduct.ExpiryPeriodInDays = product.ExpiryPeriodInDays;
            existingProduct.UpdatedAt = DateTime.UtcNow; // Set UpdatedAt before saving

            // Use ModelState.IsValid to leverage data annotations from Product.cs AND the custom uniqueness checks
            if (ModelState.IsValid)
            {
                try
                {
                    _context.SaveChanges();
                    TempData["SuccessMessage"] = "Product updated successfully!"; // Optional: for notifications
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Products.Any(e => e.Id == product.Id))
                    {
                        return NotFound(); // Product was deleted by another process
                    }
                    else
                    {
                        throw; // Re-throw if it's another concurrency issue
                    }
                }
                catch (DbUpdateException ex)
                {
                    // Catch database exceptions specifically related to unique constraints if needed
                    // This might catch cases where the unique check above passed but another process
                    // inserted the same code right before SaveChanges. Less common but possible.
                    ModelState.AddModelError("", "An error occurred while saving changes. Please try again. " + ex.Message);
                    return View("Edit", product);
                }
                return RedirectToAction("Index"); // Redirect on successful save
            }

            // If ModelState is not valid, return to the Edit view with the *product* object
            // This allows validation messages to display based on the user's last input.
            return View("Edit", product);
        }
    }
}