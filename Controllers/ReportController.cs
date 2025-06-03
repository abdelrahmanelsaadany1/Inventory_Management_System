using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inventory_Management_System.Models;
using Inventory_Management_System.ViewModels;

namespace Inventory_Management_System.Controllers
{
    public class ReportController : Controller
    {
        private readonly InventoryDbContext _context = new InventoryDbContext();

        // GET: Report/ProductMovement
        public async Task<IActionResult> ProductMovement()
        {
            var viewModel = new ReportMovementViewModel
            {
                AvailableWarehouses = await _context.Warehouses.OrderBy(w => w.Name).ToListAsync()
            };
            return View(viewModel);
        }

        // POST: Report/GenerateProductMovement
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateProductMovement(ReportMovementViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableWarehouses = await _context.Warehouses.OrderBy(w => w.Name).ToListAsync();
                return View("ProductMovement", model);
            }

            var resultViewModel = new ProductMovementReportResultViewModel
            {
                ReportParameters = model,
                MovementItems = new List<ProductMovementItemViewModel>()
            };

            // Ensure EndDate includes the whole day
            DateTime endDateInclusive = model.EndDate.AddDays(1).AddSeconds(-1);

            // Fetch product movements
            var allMovementItems = new List<ProductMovementItemViewModel>();

            // 1. Inbound Movements (Supply Orders)
            var supplyOrderItems = await _context.SupplyOrderItems
                .Include(soi => soi.SupplyOrder)
                    .ThenInclude(so => so.Warehouse)
                .Include(soi => soi.Product)
                .Where(soi => soi.SupplyOrder.OrderDate >= model.StartDate && soi.SupplyOrder.OrderDate <= endDateInclusive)
                .Where(soi => model.SelectedWarehouseIds.Contains(soi.SupplyOrder.WarehouseId))
                .Select(soi => new ProductMovementItemViewModel
                {
                    ProductName = soi.Product.Name,
                    ProductCode = soi.Product.Code,
                    WarehouseName = soi.SupplyOrder.Warehouse.Name,
                    MovementDate = soi.SupplyOrder.OrderDate,
                    MovementType = "Inbound",
                    ReferenceNumber = soi.SupplyOrder.OrderNumber,
                    Quantity = soi.Quantity,
                    RelatedParty = _context.Suppliers.Where(s => s.Id == soi.SupplyOrder.SupplierId).Select(s => s.Name).FirstOrDefault(),
                    ProductionDate = soi.ProductionDate,
                    ExpiryDate = soi.ProductionDate.AddDays(soi.ExpiryPeriodInDays)
                })
                .ToListAsync();
            allMovementItems.AddRange(supplyOrderItems);

            // 2. Outbound Movements (Release Orders)
            var releaseOrderItems = await _context.ReleaseOrderItems
                .Include(roi => roi.ReleaseOrder)
                    .ThenInclude(ro => ro.Warehouse)
                .Include(roi => roi.Product)
                .Where(roi => roi.ReleaseOrder.OrderDate >= model.StartDate && roi.ReleaseOrder.OrderDate <= endDateInclusive)
                .Where(roi => model.SelectedWarehouseIds.Contains(roi.ReleaseOrder.WarehouseId))
                .Select(roi => new ProductMovementItemViewModel
                {
                    ProductName = roi.Product.Name,
                    ProductCode = roi.Product.Code,
                    WarehouseName = roi.ReleaseOrder.Warehouse.Name,
                    MovementDate = roi.ReleaseOrder.OrderDate,
                    MovementType = "Outbound",
                    ReferenceNumber = roi.ReleaseOrder.OrderNumber,
                    Quantity = roi.Quantity,
                    RelatedParty = _context.Customers.Where(c => c.ReleaseOrders.Any(ro => ro.Id == roi.ReleaseOrderId)).Select(c => c.Name).FirstOrDefault(), // Assuming ReleaseOrder links to Customer
                    ProductionDate = null, // ProductionDate/ExpiryDate not directly available on ReleaseOrderItem
                    ExpiryDate = null
                })
                .ToListAsync();
            allMovementItems.AddRange(releaseOrderItems);


            // 3. Product Transfers (Inbound and Outbound)
            // Outbound Transfers
            var outboundTransfers = await _context.ProductTransferItems
                .Include(pti => pti.ProductTransfer)
                    .ThenInclude(pt => pt.SourceWarehouse)
                .Include(pti => pti.ProductTransfer)
                    .ThenInclude(pt => pt.DestinationWarehouse)
                .Include(pti => pti.Product)
                .Where(pti => pti.ProductTransfer.TransferDate >= model.StartDate && pti.ProductTransfer.TransferDate <= endDateInclusive)
                .Where(pti => model.SelectedWarehouseIds.Contains(pti.ProductTransfer.SourceWarehouseId)) // Where this warehouse is the source
                .Select(pti => new ProductMovementItemViewModel
                {
                    ProductName = pti.Product.Name,
                    ProductCode = pti.Product.Code,
                    WarehouseName = pti.ProductTransfer.SourceWarehouse.Name, // Source warehouse
                    MovementDate = pti.ProductTransfer.TransferDate,
                    MovementType = "Transfer (Out)",
                    ReferenceNumber = pti.ProductTransfer.TransferNumber,
                    Quantity = pti.Quantity,
                    RelatedParty = pti.ProductTransfer.DestinationWarehouse.Name, // Destination warehouse
                    ProductionDate = pti.ProductionDate,
                    ExpiryDate = pti.ProductionDate.AddDays(pti.ExpiryPeriodInDays)
                })
                .ToListAsync();
            allMovementItems.AddRange(outboundTransfers);

            // Inbound Transfers
            var inboundTransfers = await _context.ProductTransferItems
                .Include(pti => pti.ProductTransfer)
                    .ThenInclude(pt => pt.SourceWarehouse)
                .Include(pti => pti.ProductTransfer)
                    .ThenInclude(pt => pt.DestinationWarehouse)
                .Include(pti => pti.Product)
                .Where(pti => pti.ProductTransfer.TransferDate >= model.StartDate && pti.ProductTransfer.TransferDate <= endDateInclusive)
                .Where(pti => model.SelectedWarehouseIds.Contains(pti.ProductTransfer.DestinationWarehouseId)) // Where this warehouse is the destination
                .Select(pti => new ProductMovementItemViewModel
                {
                    ProductName = pti.Product.Name,
                    ProductCode = pti.Product.Code,
                    WarehouseName = pti.ProductTransfer.DestinationWarehouse.Name, // Destination warehouse
                    MovementDate = pti.ProductTransfer.TransferDate,
                    MovementType = "Transfer (In)",
                    ReferenceNumber = pti.ProductTransfer.TransferNumber,
                    Quantity = pti.Quantity,
                    RelatedParty = pti.ProductTransfer.SourceWarehouse.Name, // Source warehouse
                    ProductionDate = pti.ProductionDate,
                    ExpiryDate = pti.ProductionDate.AddDays(pti.ExpiryPeriodInDays)
                })
                .ToListAsync();
            allMovementItems.AddRange(inboundTransfers);

            // Order movements by date and then type
            resultViewModel.MovementItems = allMovementItems.OrderBy(item => item.MovementDate).ToList();

            // Calculate optional summary
            resultViewModel.ProductSummary = resultViewModel.MovementItems
                .GroupBy(item => $"{item.ProductName} ({item.ProductCode})")
                .ToDictionary(
                    g => g.Key,
                    g => (decimal)g.Sum(item => item.MovementType == "Inbound" || item.MovementType == "Transfer (In)" ? item.Quantity : -item.Quantity)
                );

            return View("ProductMovementResult", resultViewModel);
        }
    }
}