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
                Items = new List<SupplyOrderItemViewModel>
                {
                    new SupplyOrderItemViewModel()
                }
            };
            return View(viewModel);
        }

      
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