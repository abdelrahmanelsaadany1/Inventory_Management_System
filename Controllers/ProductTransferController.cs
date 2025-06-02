using Inventory_Management_System.Models;
using Inventory_Management_System.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory_Management_System.Controllers
{
    public class ProductTransferController : Controller
    {
        private readonly InventoryDbContext _context = new InventoryDbContext();

        // GET: ProductTransfer/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new AddProductTransferViewModel
            {
                Warehouses = await _context.Warehouses.ToListAsync(),
                TransferDate = DateTime.Today,
                Items = new List<ProductTransferItemViewModel>
                {
                    new ProductTransferItemViewModel() // Start with one empty item row
                }
            };

            return View(viewModel);
        }

        // POST: ProductTransfer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddProductTransferViewModel viewModel)
        {
            // Filter out empty rows from the items list that might be sent from the form
            if (viewModel.Items != null)
            {
                viewModel.Items = viewModel.Items
                    .Where(item => item.ProductId > 0 && item.Quantity > 0 && item.SupplierId > 0 && item.ProductionDate != default)
                    .ToList();
            }

            // Add model error if no valid items are provided after filtering
            if (viewModel.Items == null || !viewModel.Items.Any())
            {
                ModelState.AddModelError("Items", "At least one item with a product, quantity, supplier, and production date is required for the transfer.");
            }

            // Validate that source and destination warehouses are different
            if (viewModel.SourceWarehouseId == viewModel.DestinationWarehouseId && viewModel.SourceWarehouseId != 0)
            {
                ModelState.AddModelError("DestinationWarehouseId", "Source and Destination Warehouses cannot be the same.");
            }

            if (!ModelState.IsValid)
            {
                // Reload warehouses for the main ViewModel if validation fails
                viewModel.Warehouses = await _context.Warehouses.ToListAsync();

                // **FIX: Add warehouse inventory to ViewBag so JavaScript can restore the items**
                if (viewModel.SourceWarehouseId > 0)
                {
                    var warehouseInventory = await GetWarehouseInventoryData(viewModel.SourceWarehouseId);
                    ViewBag.WarehouseInventory = System.Text.Json.JsonSerializer.Serialize(warehouseInventory);
                }
                else
                {
                    ViewBag.WarehouseInventory = "[]";
                }

                TempData["ErrorMessage"] = "Please correct the errors below.";
                return View(viewModel);
            }

            // Use a database transaction to ensure atomicity of the transfer operation
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var productTransfer = new ProductTransfer
                    {
                        TransferNumber = viewModel.TransferNumber,
                        TransferDate = viewModel.TransferDate,
                        SourceWarehouseId = viewModel.SourceWarehouseId,
                        DestinationWarehouseId = viewModel.DestinationWarehouseId,
                        CreatedAt = DateTime.Now,
                        Items = new List<ProductTransferItem>()
                    };

                    // Process each item, deduct from source, add to destination, and add to transfer record
                    foreach (var itemViewModel in viewModel.Items)
                    {
                        // Retrieve the full WarehouseProduct entry from the source to get its details
                        var sourceWarehouseProduct = await _context.WarehouseProducts
                            .FirstOrDefaultAsync(wp => wp.WarehouseId == viewModel.SourceWarehouseId &&
                                                       wp.ProductId == itemViewModel.ProductId &&
                                                       wp.SupplierId == itemViewModel.SupplierId &&
                                                       wp.ProductionDate == itemViewModel.ProductionDate);

                        if (sourceWarehouseProduct == null || sourceWarehouseProduct.Quantity < itemViewModel.Quantity)
                        {
                            var productName = (await _context.Products.FindAsync(itemViewModel.ProductId))?.Name ?? "Unknown Product";
                            var supplierName = (await _context.Suppliers.FindAsync(itemViewModel.SupplierId))?.Name ?? "Unknown Supplier";

                            ModelState.AddModelError("", $"Not enough stock for product '{productName}' (Batch: {itemViewModel.ProductionDate.ToShortDateString()}, Supplier: {supplierName}) in the source warehouse.");

                            // **FIX: Reload data for validation failure case**
                            viewModel.Warehouses = await _context.Warehouses.ToListAsync();
                            var warehouseInventory = await GetWarehouseInventoryData(viewModel.SourceWarehouseId);
                            ViewBag.WarehouseInventory = System.Text.Json.JsonSerializer.Serialize(warehouseInventory);

                            await transaction.RollbackAsync();
                            return View(viewModel);
                        }

                        // 1. Deduct from Source Warehouse
                        sourceWarehouseProduct.Quantity -= itemViewModel.Quantity;
                        if (sourceWarehouseProduct.Quantity <= 0)
                        {
                            _context.WarehouseProducts.Remove(sourceWarehouseProduct);
                        }
                        else
                        {
                            _context.WarehouseProducts.Update(sourceWarehouseProduct);
                        }

                        // 2. Add to Destination Warehouse
                        var destinationWarehouseProduct = await _context.WarehouseProducts
                            .FirstOrDefaultAsync(wp => wp.WarehouseId == viewModel.DestinationWarehouseId &&
                                                       wp.ProductId == itemViewModel.ProductId &&
                                                       wp.SupplierId == itemViewModel.SupplierId &&
                                                       wp.ProductionDate == itemViewModel.ProductionDate);

                        if (destinationWarehouseProduct == null)
                        {
                            _context.WarehouseProducts.Add(new WarehouseProduct
                            {
                                WarehouseId = viewModel.DestinationWarehouseId,
                                ProductId = itemViewModel.ProductId,
                                Quantity = itemViewModel.Quantity,
                                SupplierId = itemViewModel.SupplierId,
                                ProductionDate = itemViewModel.ProductionDate,
                                ExpiryPeriodInDays = itemViewModel.ExpiryPeriodInDays,
                                CreatedAt = DateTime.Now
                            });
                        }
                        else
                        {
                            destinationWarehouseProduct.Quantity += itemViewModel.Quantity;
                            _context.WarehouseProducts.Update(destinationWarehouseProduct);
                        }

                        // Add item to the ProductTransfer record
                        productTransfer.Items.Add(new ProductTransferItem
                        {
                            ProductId = itemViewModel.ProductId,
                            Quantity = itemViewModel.Quantity,
                            SupplierId = itemViewModel.SupplierId,
                            ProductionDate = itemViewModel.ProductionDate,
                            ExpiryPeriodInDays = itemViewModel.ExpiryPeriodInDays,
                            CreatedAt = DateTime.Now
                        });
                    }

                    _context.ProductTransfers.Add(productTransfer);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["SuccessMessage"] = "Product transfer created successfully.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    System.Diagnostics.Debug.WriteLine($"Error saving product transfer: {ex.Message}");

                    // **FIX: Reload data for exception case**
                    viewModel.Warehouses = await _context.Warehouses.ToListAsync();
                    if (viewModel.SourceWarehouseId > 0)
                    {
                        var warehouseInventory = await GetWarehouseInventoryData(viewModel.SourceWarehouseId);
                        ViewBag.WarehouseInventory = System.Text.Json.JsonSerializer.Serialize(warehouseInventory);
                    }
                    else
                    {
                        ViewBag.WarehouseInventory = "[]";
                    }

                    ModelState.AddModelError("", "An error occurred while saving the product transfer. Please check the details and try again. Error: " + ex.Message);
                    return View(viewModel);
                }
            }
        }

        // GET: ProductTransfer/Index
        public async Task<IActionResult> Index()
        {
            try
            {
                var transfers = await _context.ProductTransfers
                    .Include(t => t.SourceWarehouse)
                    .Include(t => t.DestinationWarehouse)
                    .Include(t => t.Items)
                        .ThenInclude(i => i.Product)
                    .Include(t => t.Items)
                        .ThenInclude(i => i.Supplier)
                    .OrderByDescending(t => t.CreatedAt)
                    .ToListAsync();

                return View(transfers);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading product transfers: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while loading product transfers.";
                return View(new List<ProductTransfer>());
            }
        }

        // AJAX ENDPOINT: Get items available in a specific warehouse
        [HttpGet]
        public async Task<JsonResult> GetWarehouseInventory(int warehouseId)
        {
            var inventoryItems = await GetWarehouseInventoryData(warehouseId);
            return Json(inventoryItems);
        }

        // **NEW: Helper method to get warehouse inventory data**
        private async Task<object> GetWarehouseInventoryData(int warehouseId)
        {
            var inventoryItems = await _context.WarehouseProducts
                .Where(wp => wp.WarehouseId == warehouseId && wp.Quantity > 0)
                .Include(wp => wp.Product)
                .Include(wp => wp.Supplier)
                .Select(wp => new
                {
                    ProductId = wp.ProductId,
                    ProductName = wp.Product.Name,
                    QuantityAvailable = wp.Quantity,
                    SupplierId = wp.SupplierId,
                    SupplierName = wp.Supplier.Name,
                    ProductionDate = wp.ProductionDate.ToString("yyyy-MM-dd"),
                    ExpiryPeriodInDays = wp.ExpiryPeriodInDays
                })
                .ToListAsync();

            return inventoryItems;
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