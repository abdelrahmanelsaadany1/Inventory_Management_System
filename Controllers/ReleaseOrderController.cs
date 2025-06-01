using Inventory_Management_System.Models;
using Inventory_Management_System.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Inventory_Management_System.Controllers
{
    public class ReleaseOrderController : Controller
    {
        private readonly InventoryDbContext _context = new InventoryDbContext(); // Assuming your DbContext is InventoryDbContext

        // GET: ReleaseOrder/Create
        public IActionResult Create()
        {
            var viewModel = new AddReleaseOrderViewModel
            {
                Warehouses = _context.Warehouses.ToList(),
                Suppliers = _context.Suppliers.ToList(),
                Products = _context.Products.ToList(),
                OrderDate = DateTime.Today,
                Items = new List<ReleaseOrderItemViewModel>
                {
                    new ReleaseOrderItemViewModel() // Start with one empty item row
                }
            };
            return View(viewModel);
        }

        // POST: ReleaseOrder/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AddReleaseOrderViewModel viewModel)
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

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var releaseOrder = new ReleaseOrder
                    {
                        OrderNumber = viewModel.OrderNumber,
                        OrderDate = viewModel.OrderDate,
                        WarehouseId = viewModel.WarehouseId,
                        SupplierId = viewModel.SupplierId,
                        CreatedAt = DateTime.Now,
                        Items = viewModel.Items.Select(item => new ReleaseOrderItem
                        {
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            CreatedAt = DateTime.Now
                        }).ToList()
                    };

                    _context.ReleaseOrders.Add(releaseOrder);

                    // Update WarehouseProducts: Decrease quantity
                    foreach (var item in viewModel.Items)
                    {
                        // Find the specific WarehouseProduct entry.
                        // You might need to refine this query to get the correct product based on other criteria
                        // like production date or expiry if your inventory management is more complex.
                        var warehouseProduct = _context.WarehouseProducts
                            .FirstOrDefault(wp => wp.WarehouseId == viewModel.WarehouseId && wp.ProductId == item.ProductId);

                        if (warehouseProduct == null || warehouseProduct.Quantity < item.Quantity)
                        {
                            ModelState.AddModelError("", $"Not enough stock for product ID {item.ProductId} in the selected warehouse.");
                            LoadDropdowns(viewModel);
                            transaction.Rollback();
                            return View(viewModel);
                        }

                        warehouseProduct.Quantity -= item.Quantity;
                        // If quantity becomes 0, you might want to remove the entry or mark it inactive
                        if (warehouseProduct.Quantity == 0)
                        {
                            _context.WarehouseProducts.Remove(warehouseProduct);
                        }
                        else
                        {
                            _context.WarehouseProducts.Update(warehouseProduct);
                        }
                    }

                    _context.SaveChanges();
                    transaction.Commit();

                    TempData["SuccessMessage"] = "Release order created successfully.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    System.Diagnostics.Debug.WriteLine($"Error saving release order: {ex.Message}");
                    LoadDropdowns(viewModel);
                    ModelState.AddModelError("", "An error occurred while saving the release order.");
                    return View(viewModel);
                }
            }
        }

        // GET: ReleaseOrder/Index
        public IActionResult Index()
        {
            try
            {
                var orders = _context.ReleaseOrders
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
                System.Diagnostics.Debug.WriteLine($"Error loading release orders: {ex.Message}");
                return View(new List<ReleaseOrder>());
            }
        }

        // GET: ReleaseOrder/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var releaseOrder = _context.ReleaseOrders
                .Include(r => r.Items)
                .FirstOrDefault(r => r.Id == id);

            if (releaseOrder == null)
            {
                return NotFound();
            }

            var viewModel = new AddReleaseOrderViewModel
            {
                Id = releaseOrder.Id,
                OrderNumber = releaseOrder.OrderNumber,
                OrderDate = releaseOrder.OrderDate,
                WarehouseId = releaseOrder.WarehouseId,
                SupplierId = releaseOrder.SupplierId,
                Items = releaseOrder.Items.Select(item => new ReleaseOrderItemViewModel
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                }).ToList()
            };

            LoadDropdowns(viewModel);
            return View(viewModel);
        }

        // POST: ReleaseOrder/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(AddReleaseOrderViewModel viewModel)
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

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var existingReleaseOrder = _context.ReleaseOrders
                        .Include(r => r.Items)
                        .FirstOrDefault(r => r.Id == viewModel.Id);

                    if (existingReleaseOrder == null)
                    {
                        return NotFound();
                    }

                    // Revert previous changes to WarehouseProducts before applying new ones
                    foreach (var oldItem in existingReleaseOrder.Items)
                    {
                        var warehouseProduct = _context.WarehouseProducts
                            .FirstOrDefault(wp => wp.WarehouseId == existingReleaseOrder.WarehouseId && wp.ProductId == oldItem.ProductId);
                        if (warehouseProduct != null)
                        {
                            warehouseProduct.Quantity += oldItem.Quantity; // Add back the old quantity
                            _context.WarehouseProducts.Update(warehouseProduct);
                        }
                    }

                    // Update ReleaseOrder properties
                    existingReleaseOrder.OrderNumber = viewModel.OrderNumber;
                    existingReleaseOrder.OrderDate = viewModel.OrderDate;
                    existingReleaseOrder.WarehouseId = viewModel.WarehouseId;
                    existingReleaseOrder.SupplierId = viewModel.SupplierId;
                    existingReleaseOrder.UpdatedAt = DateTime.Now;

                    // Clear existing items and add new ones
                    _context.ReleaseOrderItems.RemoveRange(existingReleaseOrder.Items);
                    existingReleaseOrder.Items = viewModel.Items.Select(item => new ReleaseOrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        CreatedAt = DateTime.Now // Or keep the original CreatedAt if it's an update
                    }).ToList();

                    // Apply new changes to WarehouseProducts: Decrease quantity
                    foreach (var newItem in viewModel.Items)
                    {
                        var warehouseProduct = _context.WarehouseProducts
                            .FirstOrDefault(wp => wp.WarehouseId == viewModel.WarehouseId && wp.ProductId == newItem.ProductId);

                        if (warehouseProduct == null || warehouseProduct.Quantity < newItem.Quantity)
                        {
                            ModelState.AddModelError("", $"Not enough stock for product ID {newItem.ProductId} in the selected warehouse.");
                            LoadDropdowns(viewModel);
                            transaction.Rollback();
                            return View(viewModel);
                        }

                        warehouseProduct.Quantity -= newItem.Quantity;
                        if (warehouseProduct.Quantity == 0)
                        {
                            _context.WarehouseProducts.Remove(warehouseProduct);
                        }
                        else
                        {
                            _context.WarehouseProducts.Update(warehouseProduct);
                        }
                    }

                    _context.ReleaseOrders.Update(existingReleaseOrder);
                    _context.SaveChanges();
                    transaction.Commit();

                    TempData["SuccessMessage"] = "Release order updated successfully.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    System.Diagnostics.Debug.WriteLine($"Error updating release order: {ex.Message}");
                    LoadDropdowns(viewModel);
                    ModelState.AddModelError("", "An error occurred while updating the release order.");
                    return View(viewModel);
                }
            }
        }


        private void LoadDropdowns(AddReleaseOrderViewModel viewModel)
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
        //new

    }
}