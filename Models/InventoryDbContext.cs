using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Inventory_Management_System.Models
{
    public class InventoryDbContext : DbContext
    {
        public InventoryDbContext() : base()
        { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=InventoryDb;Integrated Security=True;Encrypt=False;Trust Server Certificate=True");
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductTransfer> ProductTransfers { get; set; }
        public DbSet<ProductUnits> ProductUnits { get; set; }
        public DbSet<ReleaseOrder> ReleaseOrders { get; set; }
        public DbSet<ReleaseOrderItem> ReleaseOrderItems { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<SupplyOrder> SupplyOrders { get; set; }
        public DbSet<SupplyOrderItem> SupplyOrderItems { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<WarehouseProduct> WarehouseProducts { get; set; }
        public DbSet<ProductTransferItem> ProductTransferItems { get; set; }
        public DbSet<Manager> Managers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<WarehouseProduct>()
                .HasKey(wp => new { wp.WarehouseId, wp.ProductId, wp.SupplierId, wp.ProductionDate });

            modelBuilder.Entity<ProductUnits>()
                .HasKey(pu => new { pu.ProductId, pu.UnitId });

            modelBuilder.Entity<ProductTransferItem>()
                .HasKey(pti => new { pti.ProductTransferId, pti.ProductId, pti.SupplierId, pti.ProductionDate });

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Code)
                .IsUnique();

            modelBuilder.Entity<ReleaseOrder>()
                .HasIndex(ro => ro.OrderNumber)
                .IsUnique();

            modelBuilder.Entity<SupplyOrder>()
                .HasIndex(so => so.OrderNumber)
                .IsUnique();

            modelBuilder.Entity<ProductTransfer>()
                .HasIndex(pt => pt.TransferNumber)
                .IsUnique();

            modelBuilder.Entity<WarehouseProduct>()
                .Property(e => e.ProductionDate)
                .ValueGeneratedNever();

            modelBuilder.Entity<SupplyOrderItem>()
                .Property(e => e.ProductionDate)
                .ValueGeneratedNever();

            modelBuilder.Entity<ProductTransferItem>()
                .Property(e => e.ProductionDate)
                .ValueGeneratedNever();

            modelBuilder.Entity<ProductTransfer>()
                .HasOne(pt => pt.SourceWarehouse)
                .WithMany(w => w.SourceTransfers)
                .HasForeignKey(pt => pt.SourceWarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProductTransfer>()
                .HasOne(pt => pt.DestinationWarehouse)
                .WithMany(w => w.DestinationTransfers)
                .HasForeignKey(pt => pt.DestinationWarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProductTransferItem>()
                .HasOne(pti => pti.ProductTransfer)
                .WithMany(pt => pt.Items)
                .HasForeignKey(pti => pti.ProductTransferId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductTransferItem>()
                .HasOne(pti => pti.Product)
                .WithMany()
                .HasForeignKey(pti => pti.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProductTransferItem>()
                .HasOne(pti => pti.Supplier)
                .WithMany()
                .HasForeignKey(pti => pti.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProductUnits>()
                .HasOne(pu => pu.Product)
                .WithMany(p => p.ProductUnits)
                .HasForeignKey(pu => pu.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductUnits>()
                .HasOne(pu => pu.Unit)
                .WithMany(u => u.ProductUnits)
                .HasForeignKey(pu => pu.UnitId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ReleaseOrder>()
                .HasOne(ro => ro.Warehouse)
                .WithMany(w => w.ReleaseOrders)
                .HasForeignKey(ro => ro.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReleaseOrder>()
                .HasOne(ro => ro.Supplier)
                .WithMany()
                .HasForeignKey(ro => ro.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReleaseOrderItem>()
                .HasOne(roi => roi.ReleaseOrder)
                .WithMany(ro => ro.Items)
                .HasForeignKey(roi => roi.ReleaseOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SupplyOrder>()
                .HasOne(so => so.Warehouse)
                .WithMany(w => w.SupplyOrders)
                .HasForeignKey(so => so.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SupplyOrder>()
                .HasOne(so => so.Supplier)
                .WithMany(s => s.SupplyOrders)
                .HasForeignKey(so => so.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SupplyOrderItem>()
                .HasOne(soi => soi.SupplyOrder)
                .WithMany(so => so.Items)
                .HasForeignKey(soi => soi.SupplyOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WarehouseProduct>()
                .HasOne(wp => wp.Warehouse)
                .WithMany(w => w.WarehouseProducts)
                .HasForeignKey(wp => wp.WarehouseId);

            modelBuilder.Entity<WarehouseProduct>()
                .HasOne(wp => wp.Product)
                .WithMany(p => p.WarehouseProducts)
                .HasForeignKey(wp => wp.ProductId);

            modelBuilder.Entity<WarehouseProduct>()
                .HasOne(wp => wp.Supplier)
                .WithMany(s => s.WarehouseProducts)
                .HasForeignKey(wp => wp.SupplierId);

            modelBuilder.Entity<Warehouse>()
                .HasOne(w => w.Manager)
                .WithMany()
                .HasForeignKey(w => w.ManagerId);
        }
    }
}