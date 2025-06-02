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

        // GET: SupplyOrder/Create
        public IActionResult Create()
        {
            var viewModel = new AddSupplyOrderViewModel
            {
                Warehouses = _context.Warehouses.ToList(),
                Suppliers = _context.Suppliers.ToList(),
                Products = _context.Products.ToList(),
                OrderDate = DateTime.Today,
                Items = new List<SupplyOrderItemViewModel>
                {
                    new SupplyOrderItemViewModel()
                }
            };
            return View(viewModel);
        }

        // POST: SupplyOrder/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AddSupplyOrderViewModel viewModel)
        {
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

                // Add to WarehouseProducts table OR UPDATE existing entries
                foreach (var item in viewModel.Items)
                {
                    // Attempt to find an existing WarehouseProduct entry that matches the composite key
                    var existingWarehouseProduct = _context.WarehouseProducts.FirstOrDefault(wp =>
                        wp.WarehouseId == viewModel.WarehouseId &&
                        wp.ProductId == item.ProductId &&
                        wp.SupplierId == viewModel.SupplierId &&
                        wp.ProductionDate == item.ProductionDate); // Crucial part of the composite key

                    if (existingWarehouseProduct != null)
                    {
                        // If a matching entry is found, update its quantity
                        existingWarehouseProduct.Quantity += item.Quantity;
                        existingWarehouseProduct.UpdatedAt = DateTime.Now; // Update the timestamp
                        // Entity Framework Core automatically tracks changes to 'existingWarehouseProduct'
                        // so no explicit _context.Entry(existingWarehouseProduct).State = EntityState.Modified; is needed here.
                    }
                    else
                    {
                        // If no matching entry is found, create a new one
                        var warehouseProduct = new WarehouseProduct
                        {
                            WarehouseId = viewModel.WarehouseId,
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            ProductionDate = item.ProductionDate,
                            ExpiryPeriodInDays = item.ExpiryPeriodInDays,
                            SupplierId = viewModel.SupplierId,
                            CreatedAt = DateTime.Now // This was the previous fix, ensure it's here
                        };
                        _context.WarehouseProducts.Add(warehouseProduct); // Add the new entity to the context
                    }
                }

                _context.SaveChanges(); // This will now correctly save new or updated entities

                TempData["SuccessMessage"] = "Supply order created successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving: {ex.Message}");
                // Log the inner exception as well for more details if it's a DbUpdateException
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

        // GET: SupplyOrder/Index
        public IActionResult Index()
        {
            try
            {
                var orders = _context.SupplyOrders
                    .Include(o => o.Warehouse)
                    .Include(o => o.Supplier)
                    .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                    .OrderByDescending(o => o.CreatedAt)
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