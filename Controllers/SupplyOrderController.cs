using Inventory_Management_System.Models;
using Inventory_Management_System.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Inventory_Management_System.Controllers
{
    public class SupplyOrderController : Controller
    {
        private readonly InventoryDbContext _context = new InventoryDbContext();


        public IActionResult Create()
        {
            var viewModel = new AddSupplyOrderViewModel
            {
                Warehouses = _context.Warehouses.ToList(),
                Suppliers = _context.Suppliers.ToList(),
                Products = _context.Products.ToList(),
                OrderDate = DateTime.Today,
                OrderNumber = GenerateNextOrderNumber(), // Auto-generate order number
                Items = new List<SupplyOrderItemViewModel>
                {
                    new SupplyOrderItemViewModel()
                }
            };
            return View(viewModel);
        }

        // Method to generate the next available order number
        private string GenerateNextOrderNumber()
        {
            var currentYear = DateTime.Now.Year;
            var prefix = $"SO-{currentYear}-";

            // Get the highest order number for current year
            var lastOrderNumber = _context.SupplyOrders
                .Where(o => o.OrderNumber.StartsWith(prefix))
                .OrderByDescending(o => o.OrderNumber)
                .Select(o => o.OrderNumber)
                .FirstOrDefault();

            int nextNumber = 1;
            if (!string.IsNullOrEmpty(lastOrderNumber))
            {
                // Extract the number part and increment
                var numberPart = lastOrderNumber.Substring(prefix.Length);
                if (int.TryParse(numberPart, out int currentNumber))
                {
                    nextNumber = currentNumber + 1;
                }
            }

            return $"{prefix}{nextNumber:D4}"; // Format as SO-2025-0001
        }

        // API endpoint to check if order number exists
        [HttpPost]
        public JsonResult CheckOrderNumber(string orderNumber)
        {
            var exists = _context.SupplyOrders.Any(o => o.OrderNumber == orderNumber);
            var suggestion = exists ? GenerateNextOrderNumber() : null;

            return Json(new
            {
                exists = exists,
                suggestion = suggestion,
                message = exists ? $"Order number '{orderNumber}' already exists. Try: {suggestion}" : "Order number is available"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AddSupplyOrderViewModel viewModel)
        {
            // Check for duplicate order number
            if (_context.SupplyOrders.Any(o => o.OrderNumber == viewModel.OrderNumber))
            {
                var suggestion = GenerateNextOrderNumber();
                ModelState.AddModelError("OrderNumber", $"Order number '{viewModel.OrderNumber}' already exists. Suggested: {suggestion}");
            }

            if (viewModel.Items != null)
            {
                viewModel.Items = viewModel.Items
                    .Where(item => item.ProductId > 0 && item.Quantity > 0)
                    .ToList();
            }

            if (viewModel.Items == null || !viewModel.Items.Any())
            {
                ModelState.AddModelError("Items", "At least one item is required.");
            }

            if (!ModelState.IsValid)
            {
                LoadDropdowns(viewModel);
                TempData["ErrorMessage"] = "Please correct the errors below.";
                return View(viewModel);
            }

            try
            {
                var supplyOrder = new SupplyOrder
                {
                    OrderNumber = viewModel.OrderNumber,
                    OrderDate = viewModel.OrderDate,
                    WarehouseId = viewModel.WarehouseId,
                    SupplierId = viewModel.SupplierId,
                    CreatedAt = DateTime.Now,
                    Items = viewModel.Items.Select(item => new SupplyOrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        ProductionDate = item.ProductionDate,
                        ExpiryPeriodInDays = item.ExpiryPeriodInDays,
                        CreatedAt = DateTime.Now
                    }).ToList()
                };

                _context.SupplyOrders.Add(supplyOrder);


                foreach (var item in viewModel.Items)
                {

                    var existingWarehouseProduct = _context.WarehouseProducts.FirstOrDefault(wp =>
                        wp.WarehouseId == viewModel.WarehouseId &&
                        wp.ProductId == item.ProductId &&
                        wp.SupplierId == viewModel.SupplierId &&
                        wp.ProductionDate == item.ProductionDate);

                    if (existingWarehouseProduct != null)
                    {

                        existingWarehouseProduct.Quantity += item.Quantity;
                        existingWarehouseProduct.UpdatedAt = DateTime.Now;

                    }
                    else
                    {

                        var warehouseProduct = new WarehouseProduct
                        {
                            WarehouseId = viewModel.WarehouseId,
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            ProductionDate = item.ProductionDate,
                            ExpiryPeriodInDays = item.ExpiryPeriodInDays,
                            SupplierId = viewModel.SupplierId,
                            CreatedAt = DateTime.Now
                        };
                        _context.WarehouseProducts.Add(warehouseProduct);
                    }
                }

                _context.SaveChanges();

                TempData["SuccessMessage"] = "Supply order created successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving: {ex.Message}");

                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                LoadDropdowns(viewModel);
                ModelState.AddModelError("", "An error occurred while saving the supply order. Please check debug output for details.");
                TempData["ErrorMessage"] = "An error occurred while saving the supply order. Please try again.";
                return View(viewModel);
            }
        }


        public IActionResult Index()
        {
            try
            {
                var orders = _context.SupplyOrders
                    .Include(o => o.Warehouse)
                    .Include(o => o.Supplier)
                    .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                    .OrderBy(o => o.Id)
                    .ToList();

                return View(orders);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading orders: {ex.Message}");
                return View(new List<SupplyOrder>());
            }
        }

        private void LoadDropdowns(AddSupplyOrderViewModel viewModel)
        {
            viewModel.Warehouses = _context.Warehouses.ToList();
            viewModel.Suppliers = _context.Suppliers.ToList();
            viewModel.Products = _context.Products.ToList();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}