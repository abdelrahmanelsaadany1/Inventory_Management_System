// Controllers/ProductTransferController.cs

using Inventory_Management_System.Models;
using Inventory_Management_System.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory_Management_System.Controllers
{
    public class ProductTransferController : Controller
    {
        // Initialize DbContext directly as per your provided code
        // IMPORTANT: In a real application, use Dependency Injection for DbContext
        private readonly InventoryDbContext _context = new InventoryDbContext();

        // Helper to populate dropdowns for the form
        private void PopulateDropdowns(ProductTransferCreateViewModel model)
        {
            // Always repopulate the dropdowns - this is crucial for error handling
            model.AllWarehouses = new SelectList(_context.Warehouses.OrderBy(w => w.Name), "Id", "Name", model.SourceWarehouseId);

            // Populate AllProducts with Product Name AND ExpiryPeriodInDays for JS lookup
            model.AllProducts = new SelectList(
                _context.Products.Select(p => new { Id = p.Id, Name = p.Name, ExpiryPeriodInDays = p.ExpiryPeriodInDays }).OrderBy(p => p.Name),
                "Id", "Name"
            );

            model.AllSuppliers = new SelectList(_context.Suppliers.OrderBy(s => s.Name), "Id", "Name");
        }

        // GET: ProductTransfer/Create
        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new ProductTransferCreateViewModel();
            PopulateDropdowns(viewModel);

            // Initialize with empty list for the new UI
            viewModel.Items = new List<ProductTransferItemViewModel>();

            return View(viewModel);
        }

        // POST: ProductTransfer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductTransferCreateViewModel viewModel)
        {
            // CRITICAL: Always repopulate dropdowns FIRST, before any (now removed) validation
            PopulateDropdowns(viewModel);

            // --- ALL VALIDATION LOGIC IS COMMENTED OUT FOR DEBUGGING ---

            // // Basic validation: Source and Destination warehouses must be different
            // if (viewModel.SourceWarehouseId == viewModel.DestinationWarehouseId && viewModel.SourceWarehouseId != 0)
            // {
            //     ModelState.AddModelError(nameof(viewModel.DestinationWarehouseId), "Source and Destination warehouses cannot be the same.");
            // }

            // // Ensure there's at least one item selected for transfer
            // if (viewModel.Items == null || !viewModel.Items.Any())
            // {
            //     ModelState.AddModelError("", "At least one product item must be specified for transfer.");
            // }
            // else
            // {
            //     // Server-side validation for each item in the transfer
            //     for (int i = 0; i < viewModel.Items.Count; i++)
            //     {
            //         var itemViewModel = viewModel.Items[i];

            //         // Ensure the Index is set correctly for validation error display
            //         itemViewModel.Index = i;

            //         // Find the specific batch in the source warehouse to validate quantity and get expiry
            //         var sourceBatch = await _context.WarehouseProducts
            //             .AsNoTracking()
            //             .Include(wp => wp.Product)
            //             .Include(wp => wp.Supplier)
            //             .FirstOrDefaultAsync(wp =>
            //                 wp.WarehouseId == viewModel.SourceWarehouseId &&
            //                 wp.ProductId == itemViewModel.ProductId &&
            //                 wp.SupplierId == itemViewModel.SupplierId &&
            //                 wp.ProductionDate.Date == itemViewModel.ProductionDate.Date);

            //         if (sourceBatch == null)
            //         {
            //             // Debug: Let's see what's actually in the warehouse for this combination
            //             var debugBatches = await _context.WarehouseProducts
            //                 .AsNoTracking()
            //                 .Include(wp => wp.Product)
            //                 .Include(wp => wp.Supplier)
            //                 .Where(wp => wp.WarehouseId == viewModel.SourceWarehouseId &&
            //                                 wp.ProductId == itemViewModel.ProductId &&
            //                                 wp.SupplierId == itemViewModel.SupplierId)
            //                 .Select(wp => new {
            //                     wp.ProductionDate,
            //                     wp.Quantity,
            //                     ProductName = wp.Product.Name,
            //                     SupplierName = wp.Supplier.Name
            //                 })
            //                 .ToListAsync();

            //             var debugInfo = debugBatches.Any()
            //                 ? $" Available batches for this Product+Supplier: {string.Join(", ", debugBatches.Select(b => $"{b.ProductionDate.ToShortDateString()} (Qty: {b.Quantity})"))}"
            //                 : " No batches found for this Product+Supplier combination in the source warehouse.";

            //             ModelState.AddModelError($"Items[{i}].ProductId",
            //                 $"Selected product batch (Product: {itemViewModel.ProductId}, Supplier: {itemViewModel.SupplierId}, Date: {itemViewModel.ProductionDate.ToShortDateString()}) not found in source warehouse.{debugInfo}");
            //             continue;
            //         }

            //         if (itemViewModel.Quantity > sourceBatch.Quantity)
            //         {
            //             ModelState.AddModelError($"Items[{i}].Quantity",
            //                 $"Insufficient stock for Product '{sourceBatch.Product.Name}' " +
            //                 $"(Batch: {sourceBatch.Supplier?.Name}, {sourceBatch.ProductionDate.ToShortDateString()}) " +
            //                 $"in the Source Warehouse. Available: {sourceBatch.Quantity}. Requested: {itemViewModel.Quantity}");
            //         }

            //         // Set the ExpiryPeriodInDays from the source batch data
            //         itemViewModel.ExpiryPeriodInDays = sourceBatch.ExpiryPeriodInDays;
            //     }
            // }

            // // This entire block is now commented out
            // if (!ModelState.IsValid)
            // {
            //     // Ensure items have their correct Index for client-side JavaScript if validation fails
            //     for (int i = 0; i < viewModel.Items.Count; i++)
            //     {
            //         viewModel.Items[i].Index = i;
            //     }

            //     // Return the view with the populated dropdowns and validation errors
            //     return View(viewModel);
            // }

            // If validation passes (or is bypassed), proceed with the transfer
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // 1. Create ProductTransfer header
                    var productTransfer = new ProductTransfer
                    {
                        TransferNumber = viewModel.TransferNumber ?? $"TRN-{DateTime.Now.Ticks}", // Provide a default if null due to no validation
                        TransferDate = viewModel.TransferDate,
                        SourceWarehouseId = viewModel.SourceWarehouseId,
                        DestinationWarehouseId = viewModel.DestinationWarehouseId,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    _context.ProductTransfers.Add(productTransfer);
                    await _context.SaveChangesAsync();

                    // 2. Add ProductTransferItems and update WarehouseProduct inventory
                    // Ensure viewModel.Items is not null
                    foreach (var itemViewModel in viewModel.Items ?? new List<ProductTransferItemViewModel>())
                    {
                        // Decrement stock in Source Warehouse
                        // IMPORTANT: This will throw an exception if sourceBatch is null or quantity is insufficient,
                        // as we removed validation. Be prepared for database errors.
                        var sourceWarehouseProduct = await _context.WarehouseProducts
                            .FirstAsync(wp =>
                                wp.WarehouseId == viewModel.SourceWarehouseId &&
                                wp.ProductId == itemViewModel.ProductId &&
                                wp.SupplierId == itemViewModel.SupplierId &&
                                wp.ProductionDate.Date == itemViewModel.ProductionDate.Date);

                        sourceWarehouseProduct.Quantity -= itemViewModel.Quantity;
                        sourceWarehouseProduct.UpdatedAt = DateTime.Now;

                        // Increment stock in Destination Warehouse OR create new entry
                        var destinationWarehouseProduct = await _context.WarehouseProducts
                            .FirstOrDefaultAsync(wp =>
                                wp.WarehouseId == viewModel.DestinationWarehouseId &&
                                wp.ProductId == itemViewModel.ProductId &&
                                wp.SupplierId == itemViewModel.SupplierId &&
                                wp.ProductionDate.Date == itemViewModel.ProductionDate.Date);

                        if (destinationWarehouseProduct == null)
                        {
                            // Create new WarehouseProduct entry in destination
                            _context.WarehouseProducts.Add(new WarehouseProduct
                            {
                                WarehouseId = viewModel.DestinationWarehouseId,
                                ProductId = itemViewModel.ProductId,
                                SupplierId = itemViewModel.SupplierId,
                                Quantity = itemViewModel.Quantity,
                                ProductionDate = itemViewModel.ProductionDate,
                                ExpiryPeriodInDays = itemViewModel.ExpiryPeriodInDays, // Assuming this comes correctly from client
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now
                            });
                        }
                        else
                        {
                            // Increment existing WarehouseProduct entry in destination
                            destinationWarehouseProduct.Quantity += itemViewModel.Quantity;
                            destinationWarehouseProduct.UpdatedAt = DateTime.Now;
                        }

                        // Add the ProductTransferItem
                        productTransfer.Items.Add(new ProductTransferItem
                        {
                            ProductTransferId = productTransfer.Id,
                            ProductId = itemViewModel.ProductId,
                            Quantity = itemViewModel.Quantity,
                            SupplierId = itemViewModel.SupplierId,
                            ProductionDate = itemViewModel.ProductionDate,
                            ExpiryPeriodInDays = itemViewModel.ExpiryPeriodInDays, // Assuming this comes correctly from client
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        });
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["SuccessMessage"] = $"Product Transfer '{productTransfer.TransferNumber}' created successfully and inventory updated.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    // Still add a model error so it's visible on the page
                    ModelState.AddModelError("", "An error occurred during the transfer: " + ex.Message + " (Validation bypassed, this is likely a database constraint violation or missing data from UI)");

                    // IMPORTANT: Repopulate dropdowns again after exception
                    PopulateDropdowns(viewModel);

                    // Ensure items have their correct Index for client-side JavaScript
                    // This is still needed for re-rendering the view with the items already selected if an error occurs.
                    for (int i = 0; i < viewModel.Items.Count; i++)
                    {
                        viewModel.Items[i].Index = i;
                    }

                    return View(viewModel);
                }
            }
        }

        // GET: ProductTransfer/GetWarehouseProducts?warehouseId=X
        [HttpGet]
        public async Task<IActionResult> GetWarehouseProducts(int warehouseId)
        {
            if (warehouseId == 0)
            {
                return Json(new List<object>());
            }

            var warehouseProducts = await _context.WarehouseProducts
                .Where(wp => wp.WarehouseId == warehouseId && wp.Quantity > 0)
                .Include(wp => wp.Product)
                .Include(wp => wp.Supplier)
                .Select(wp => new
                {
                    wp.ProductId,
                    ProductName = wp.Product.Name,
                    wp.SupplierId,
                    SupplierName = wp.Supplier.Name,
                    wp.ProductionDate,
                    wp.Quantity,
                    wp.ExpiryPeriodInDays
                })
                .ToListAsync();

            return Json(warehouseProducts);
        }

        // GET: ProductTransfer
        public async Task<IActionResult> Index()
        {
            var productTransfers = await _context.ProductTransfers
                                                .Include(pt => pt.SourceWarehouse)
                                                .Include(pt => pt.DestinationWarehouse)
                                                .OrderByDescending(pt => pt.TransferDate)
                                                .ThenByDescending(pt => pt.CreatedAt)
                                                .ToListAsync();
            return View(productTransfers);
        }

        // GET: ProductTransfer/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productTransfer = await _context.ProductTransfers
                .Include(pt => pt.SourceWarehouse)
                .Include(pt => pt.DestinationWarehouse)
                .Include(pt => pt.Items)
                    .ThenInclude(pti => pti.Product)
                .Include(pt => pt.Items)
                    .ThenInclude(pti => pti.Supplier)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (productTransfer == null)
            {
                return NotFound();
            }

            return View(productTransfer);
        }

        // Dispose method to clean up DbContext
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