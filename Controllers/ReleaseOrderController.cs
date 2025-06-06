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
        private readonly InventoryDbContext _context = new InventoryDbContext();

        public IActionResult Create()
        {
            var viewModel = new AddReleaseOrderViewModel
            {
                Warehouses = _context.Warehouses.ToList(),
                Suppliers = _context.Suppliers.ToList(),
                Products = _context.Products.ToList(),
                OrderDate = DateTime.Today,
                OrderNumber = GenerateUniqueOrderNumber(),
                Items = new List<ReleaseOrderItemViewModel>
                {
                    new ReleaseOrderItemViewModel()
                }
            };
            return View(viewModel);
        }


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
                        Id = viewModel.Id,
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


                    foreach (var item in viewModel.Items)
                    {

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


        public IActionResult Index()
        {
            try
            {
                var orders = _context.ReleaseOrders
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
                System.Diagnostics.Debug.WriteLine($"Error loading release orders: {ex.Message}");
                return View(new List<ReleaseOrder>());
            }
        }


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


                    foreach (var oldItem in existingReleaseOrder.Items)
                    {
                        var warehouseProduct = _context.WarehouseProducts
                            .FirstOrDefault(wp => wp.WarehouseId == existingReleaseOrder.WarehouseId && wp.ProductId == oldItem.ProductId);
                        if (warehouseProduct != null)
                        {
                            warehouseProduct.Quantity += oldItem.Quantity;
                            _context.WarehouseProducts.Update(warehouseProduct);
                        }
                    }


                    existingReleaseOrder.OrderNumber = viewModel.OrderNumber;
                    existingReleaseOrder.OrderDate = viewModel.OrderDate;
                    existingReleaseOrder.WarehouseId = viewModel.WarehouseId;
                    existingReleaseOrder.SupplierId = viewModel.SupplierId;
                    existingReleaseOrder.UpdatedAt = DateTime.Now;


                    _context.ReleaseOrderItems.RemoveRange(existingReleaseOrder.Items);
                    existingReleaseOrder.Items = viewModel.Items.Select(item => new ReleaseOrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        CreatedAt = DateTime.Now
                    }).ToList();


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

        private string GenerateUniqueOrderNumber()
        {
            string prefix = "RO";
            string datePart = DateTime.Now.ToString("yyyyMMdd");

            // Get the highest order number for today
            string todayPrefix = $"{prefix}{datePart}";
            var existingOrders = _context.ReleaseOrders
                .Where(r => r.OrderNumber.StartsWith(todayPrefix))
                .Select(r => r.OrderNumber)
                .ToList();

            int nextSequence = 1;
            if (existingOrders.Any())
            {
                var sequences = existingOrders
                    .Select(orderNum =>
                    {
                        string sequencePart = orderNum.Substring(todayPrefix.Length);
                        return int.TryParse(sequencePart, out int seq) ? seq : 0;
                    })
                    .Where(seq => seq > 0);

                if (sequences.Any())
                {
                    nextSequence = sequences.Max() + 1;
                }
            }

            return $"{todayPrefix}{nextSequence:D3}";
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