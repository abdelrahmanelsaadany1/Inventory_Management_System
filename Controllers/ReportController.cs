using Inventory_Management_System.Models;
using Inventory_Management_System.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Inventory_Management_System.Controllers
{
    public class ReportController : Controller
    {
        private readonly InventoryDbContext _context = new InventoryDbContext();

       
        public IActionResult ItemMovement()
        {
            var viewModel = new ItemMovementReportViewModel
            {
                StartDate = DateTime.Today.AddDays(-30), 
                EndDate = DateTime.Today,
                AvailableWarehouses = _context.Warehouses
                    .Select(w => new WarehouseDto
                    {
                        Id = w.Id,
                        Name = w.Name
                    })
                    .ToList()
            };

            return View(viewModel);
        }

    
        [HttpPost]
        public IActionResult ItemMovement(ItemMovementReportViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableWarehouses = _context.Warehouses
                    .Select(w => new WarehouseDto
                    {
                        Id = w.Id,
                        Name = w.Name
                    })
                    .ToList();
                return View(model);
            }

           
            var warehouseIds = model.SelectedWarehouseIds?.Any() == true 
                ? model.SelectedWarehouseIds 
                : _context.Warehouses.Select(w => w.Id).ToList();

            var initialQuantities = _context.WarehouseProducts
                .Where(wp => warehouseIds.Contains(wp.WarehouseId))
                .GroupBy(wp => new { wp.ProductId, wp.WarehouseId })
                .Select(g => new
                {
                    ProductId = g.Key.ProductId,
                    WarehouseId = g.Key.WarehouseId,
                    Quantity = g.Sum(wp => wp.Quantity)
                })
                .ToDictionary(x => (x.ProductId, x.WarehouseId), x => x.Quantity);

           
            var supplyQuantities = _context.SupplyOrderItems
                .Include(soi => soi.SupplyOrder)
                .Where(soi => soi.SupplyOrder.OrderDate >= model.StartDate 
                    && soi.SupplyOrder.OrderDate <= model.EndDate
                    && warehouseIds.Contains(soi.SupplyOrder.WarehouseId))
                .GroupBy(soi => new { soi.ProductId, soi.SupplyOrder.WarehouseId })
                .Select(g => new
                {
                    ProductId = g.Key.ProductId,
                    WarehouseId = g.Key.WarehouseId,
                    Quantity = g.Sum(soi => soi.Quantity)
                })
                .ToDictionary(x => (x.ProductId, x.WarehouseId), x => x.Quantity);

           
            var releaseQuantities = _context.ReleaseOrderItems
                .Include(roi => roi.ReleaseOrder)
                .Where(roi => roi.ReleaseOrder.OrderDate >= model.StartDate 
                    && roi.ReleaseOrder.OrderDate <= model.EndDate
                    && warehouseIds.Contains(roi.ReleaseOrder.WarehouseId))
                .GroupBy(roi => new { roi.ProductId, roi.ReleaseOrder.WarehouseId })
                .Select(g => new
                {
                    ProductId = g.Key.ProductId,
                    WarehouseId = g.Key.WarehouseId,
                    Quantity = g.Sum(roi => roi.Quantity)
                })
                .ToDictionary(x => (x.ProductId, x.WarehouseId), x => x.Quantity);

            var products = _context.Products
                .Where(p => initialQuantities.Keys.Select(k => k.ProductId).Contains(p.Id)
                    || supplyQuantities.Keys.Select(k => k.ProductId).Contains(p.Id)
                    || releaseQuantities.Keys.Select(k => k.ProductId).Contains(p.Id))
                .ToDictionary(p => p.Id, p => p);

            var warehouses = _context.Warehouses
                .Where(w => warehouseIds.Contains(w.Id))
                .ToDictionary(w => w.Id, w => w);

           
            var results = new List<ItemMovementResult>();

            foreach (var product in products.Values)
            {
                foreach (var warehouse in warehouses.Values)
                {
                    var key = (product.Id, warehouse.Id);
                    var initial = initialQuantities.GetValueOrDefault(key, 0);
                    var incoming = supplyQuantities.GetValueOrDefault(key, 0);
                    var outgoing = releaseQuantities.GetValueOrDefault(key, 0);

                    results.Add(new ItemMovementResult
                    {
                        ItemCode = product.Code,
                        ItemName = product.Name,
                        WarehouseName = warehouse.Name,
                        InitialQuantity = initial,
                        IncomingQuantity = incoming,
                        OutgoingQuantity = outgoing,
                        FinalQuantity = initial + incoming - outgoing
                    });
                }
            }

           
            model.Results = results;
            model.AvailableWarehouses = warehouses.Values
                .Select(w => new WarehouseDto
                {
                    Id = w.Id,
                    Name = w.Name
                })
                .ToList();

            return View(model);
        }
    }
} 