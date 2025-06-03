// Controllers/WarehouseActivityReportController.cs
using Inventory_Management_System.Models;
using Inventory_Management_System.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Inventory_Management_System.Controllers
{
    public class WarehouseActivityReportController : Controller
    {
        private readonly InventoryDbContext _context = new InventoryDbContext();

        private void PopulateDropdowns(WarehouseActivityReportRequestViewModel model)
        {
            model.AllWarehouses = new SelectList(_context.Warehouses.OrderBy(w => w.Name), "Id", "Name", model.SelectedWarehouseId);
        }

        [HttpGet]
        public IActionResult Index()
        {
            var requestModel = new WarehouseActivityReportRequestViewModel
            {
                StartDate = DateTime.Today.AddMonths(-1),
                EndDate = DateTime.Today
            };
            PopulateDropdowns(requestModel);
            requestModel.ReportData = new List<WarehouseActivityReportViewModel>();
            return View(requestModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(WarehouseActivityReportRequestViewModel requestModel)
        {
            PopulateDropdowns(requestModel);

            if (requestModel.StartDate.HasValue && requestModel.EndDate.HasValue && requestModel.StartDate > requestModel.EndDate)
            {
                ModelState.AddModelError("", "Start Date cannot be after End Date.");
            }

            if (!ModelState.IsValid)
            {
                requestModel.ReportData = new List<WarehouseActivityReportViewModel>();
                return View(requestModel);
            }

            if (requestModel.SelectedWarehouseId != 0)
            {
                requestModel.ReportData = GetWarehouseActivityReport(
                    requestModel.SelectedWarehouseId,
                    requestModel.StartDate,
                    requestModel.EndDate
                );
            }
            else
            {
                requestModel.ReportData = new List<WarehouseActivityReportViewModel>();
                ModelState.AddModelError("SelectedWarehouseId", "Please select a warehouse to generate the report.");
            }

            return View(requestModel);
        }

        private List<WarehouseActivityReportViewModel> GetWarehouseActivityReport(
            int warehouseId,
            DateTime? startDate,
            DateTime? endDate)
        {
            var query = _context.WarehouseProducts
                .Include(wp => wp.Product)
                .Include(wp => wp.Supplier)
                .Include(wp => wp.Warehouse)
                .Where(wp => wp.WarehouseId == warehouseId);

            if (startDate.HasValue)
            {
                query = query.Where(wp => wp.CreatedAt.Date >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                query = query.Where(wp => wp.CreatedAt.Date <= endDate.Value.Date);
            }

         
            query = query.OrderBy(wp => wp.Product.Name)
                         .ThenBy(wp => wp.CreatedAt); 

            var reportData = query
                .Select(wp => new WarehouseActivityReportViewModel
                {
                    ProductName = wp.Product.Name,
                    ProductCode = wp.Product.Code,
                    SupplierName = wp.Supplier.Name,
                    QuantityInWarehouse = wp.Quantity,
                    ProductionDate = wp.ProductionDate,
                    EntryDateIntoWarehouse = wp.CreatedAt,
                    DaysInWarehouse = (int)(DateTime.Today - (wp.UpdatedAt ?? wp.CreatedAt).Date).TotalDays
                })
                .ToList();

            return reportData;
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