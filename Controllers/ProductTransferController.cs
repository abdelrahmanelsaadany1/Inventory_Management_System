using Inventory_Management_System.Models;
using Inventory_Management_System.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory_Management_System.Controllers
{
    public class ProductTransferController : Controller
    {
        private readonly InventoryDbContext _context = new InventoryDbContext();
       
        public async Task<IActionResult> Index()
        {
            var transfers = await _context.ProductTransfers
                .Include(pt => pt.SourceWarehouse)
                .Include(pt => pt.DestinationWarehouse)
                .Include(pt => pt.Items)
                .OrderByDescending(pt => pt.CreatedAt)
                .ToListAsync();

            return View(transfers);
        }

        
        [HttpGet]
        public async Task<IActionResult> GetDetails(int id)
        {
            var transfer = await _context.ProductTransfers
                .Include(pt => pt.SourceWarehouse)
                .Include(pt => pt.DestinationWarehouse)
                .Include(pt => pt.Items)
                    .ThenInclude(i => i.Product)
                .Include(pt => pt.Items)
                    .ThenInclude(i => i.Supplier)
                .FirstOrDefaultAsync(pt => pt.Id == id);

            if (transfer == null)
            {
                return NotFound();
            }

            var result = new
            {
                transferNumber = transfer.TransferNumber,
                transferDate = transfer.TransferDate,
                createdAt = transfer.CreatedAt,
                sourceWarehouseName = transfer.SourceWarehouse.Name,
                destinationWarehouseName = transfer.DestinationWarehouse.Name,
                items = transfer.Items.Select(item => new
                {
                    productName = item.Product.Name,
                    supplierName = item.Supplier.Name,
                    productionDate = item.ProductionDate,
                    expiryPeriodInDays = item.ExpiryPeriodInDays,
                    quantity = item.Quantity
                })
            };

            return Json(result);
        }

       
        public async Task<IActionResult> Create()
        {
            var warehouses = await _context.Warehouses.OrderBy(w => w.Name).ToListAsync();
            ViewBag.Warehouses = new SelectList(warehouses, "Id", "Name");
            
            return View(new ProductTransferViewModel
            {
                TransferDate = DateTime.Today
            });
        }

      
        [HttpGet]
        public async Task<IActionResult> GetWarehouseProducts(int warehouseId)
        {
            try
            {
                var products = await _context.WarehouseProducts
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
                        wp.ExpiryPeriodInDays,
                        wp.Quantity
                    })
                    .OrderBy(p => p.ProductName)
                    .ToListAsync();

                return Json(products);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductTransferViewModel model)
        {
            if (model.SourceWarehouseId == model.DestinationWarehouseId)
            {
                ModelState.AddModelError("", "لا يمكن أن يكون المخزن المصدر والمقصد نفس المخزن");
            }

            if (ModelState.IsValid)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var transfer = new ProductTransfer
                    {
                        TransferNumber = $"TR-{DateTime.Now:yyyyMMdd-HHmmss}",
                        TransferDate = model.TransferDate,
                        SourceWarehouseId = model.SourceWarehouseId,
                        DestinationWarehouseId = model.DestinationWarehouseId,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _context.Add(transfer);
                    await _context.SaveChangesAsync();

                    foreach (var item in model.Items)
                    {
       
                        var sourceProduct = await _context.WarehouseProducts
                            .FirstOrDefaultAsync(wp => 
                                wp.WarehouseId == model.SourceWarehouseId &&
                                wp.ProductId == item.ProductId &&
                                wp.SupplierId == item.SupplierId &&
                                wp.ProductionDate == item.ProductionDate);

                        if (sourceProduct == null || sourceProduct.Quantity < item.Quantity)
                        {
                            throw new Exception($"الكمية غير متوفرة للمنتج {item.ProductName}");
                        }

                 
                        sourceProduct.Quantity -= item.Quantity;
                        _context.Update(sourceProduct);

                        
                        var destProduct = await _context.WarehouseProducts
                            .FirstOrDefaultAsync(wp =>
                                wp.WarehouseId == model.DestinationWarehouseId &&
                                wp.ProductId == item.ProductId &&
                                wp.SupplierId == item.SupplierId &&
                                wp.ProductionDate == item.ProductionDate);

                        if (destProduct == null)
                        {
                            destProduct = new WarehouseProduct
                            {
                                WarehouseId = model.DestinationWarehouseId,
                                ProductId = item.ProductId,
                                SupplierId = item.SupplierId,
                                ProductionDate = item.ProductionDate,
                                ExpiryPeriodInDays = item.ExpiryPeriodInDays,
                                Quantity = item.Quantity,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            };
                            _context.Add(destProduct);
                        }
                        else
                        {
                            destProduct.Quantity += item.Quantity;
                            destProduct.UpdatedAt = DateTime.UtcNow;
                            _context.Update(destProduct);
                        }

                       
                        var transferItem = new ProductTransferItem
                        {
                            ProductTransferId = transfer.Id,
                            ProductId = item.ProductId,
                            SupplierId = item.SupplierId,
                            ProductionDate = item.ProductionDate,
                            ExpiryPeriodInDays = item.ExpiryPeriodInDays,
                            Quantity = item.Quantity,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                        _context.Add(transferItem);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    
                    TempData["SuccessMessage"] = "تم تنفيذ عملية التحويل بنجاح";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", $"حدث خطأ أثناء التحويل: {ex.Message}");
                }
            }

            var warehouses = await _context.Warehouses.OrderBy(w => w.Name).ToListAsync();
            ViewBag.Warehouses = new SelectList(warehouses, "Id", "Name");
            return View(model);
        }

        
        public async Task<IActionResult> MovementReport(DateTime? fromDate, DateTime? toDate, int? productId, int? warehouseId)
        {
            ViewBag.Products = new SelectList(await _context.Products.OrderBy(p => p.Name).ToListAsync(), "Id", "Name");
            ViewBag.Warehouses = new SelectList(await _context.Warehouses.OrderBy(w => w.Name).ToListAsync(), "Id", "Name");

            var model = new ProductMovementReportViewModel
            {
                FromDate = fromDate,
                ToDate = toDate,
                ProductId = productId,
                WarehouseId = warehouseId
            };

            if (fromDate.HasValue || toDate.HasValue || productId.HasValue || warehouseId.HasValue)
            {
                var query = _context.ProductTransferItems
                    .Include(pti => pti.ProductTransfer)
                        .ThenInclude(pt => pt.SourceWarehouse)
                    .Include(pti => pti.ProductTransfer)
                        .ThenInclude(pt => pt.DestinationWarehouse)
                    .Include(pti => pti.Product)
                    .Include(pti => pti.Supplier)
                    .AsQueryable();

                if (fromDate.HasValue)
                {
                    query = query.Where(pti => pti.ProductTransfer.TransferDate >= fromDate.Value.Date);
                }

                if (toDate.HasValue)
                {
                    query = query.Where(pti => pti.ProductTransfer.TransferDate <= toDate.Value.Date.AddDays(1).AddSeconds(-1));
                }

                if (productId.HasValue)
                {
                    query = query.Where(pti => pti.ProductId == productId.Value);
                }

                if (warehouseId.HasValue)
                {
                    query = query.Where(pti => 
                        pti.ProductTransfer.SourceWarehouseId == warehouseId.Value || 
                        pti.ProductTransfer.DestinationWarehouseId == warehouseId.Value);
                }

                
                var movements = await query
                    .OrderByDescending(pti => pti.ProductTransfer.TransferDate)
                    .Select(pti => new ProductMovementItem
                    {
                        Date = pti.ProductTransfer.TransferDate,
                        TransferNumber = pti.ProductTransfer.TransferNumber,
                        ProductName = pti.Product.Name,
                        SupplierName = pti.Supplier.Name,
                        SourceWarehouse = pti.ProductTransfer.SourceWarehouse.Name,
                        DestinationWarehouse = pti.ProductTransfer.DestinationWarehouse.Name,
                        Quantity = pti.Quantity,
                        ProductionDate = pti.ProductionDate,
                        ExpiryPeriodInDays = pti.ExpiryPeriodInDays
                    })
                    .ToListAsync();

                model.Movements = movements;
            }

            return View(model);
        }
    }
}
