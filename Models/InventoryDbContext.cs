using Microsoft.EntityFrameworkCore;
using System.Linq; // Make sure this is present

namespace Inventory_Management_System.Models
{
    public class InventoryDbContext : DbContext
    {
        public InventoryDbContext() : base()
        { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=InventoryDb;Integrated Security=True;Encrypt=False;Trust Server Certificate=True");
            // base.OnConfiguring(optionsBuilder); // No need to call base.OnConfiguring if you call base() in constructor
        }

        // --- All your DbSets ---
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductTransfer> ProductTransfers { get; set; } // Header
        public DbSet<ProductUnits> ProductUnits { get; set; }
        public DbSet<ReleaseOrder> ReleaseOrders { get; set; }
        public DbSet<ReleaseOrderItem> ReleaseOrderItems { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<SupplyOrder> SupplyOrders { get; set; }
        public DbSet<SupplyOrderItem> SupplyOrderItems { get; set; }
        public DbSet<Unit> Units { get; set; } // Assuming you have a Unit model
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<WarehouseProduct> WarehouseProducts { get; set; }
        public DbSet<ProductTransferItem> ProductTransferItems { get; set; } // Detail
        public DbSet<Manager> Managers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- 1. Define Primary Keys (Composite Keys go first for clarity) ---

            // Composite Key for WarehouseProduct (MUST include ProductionDate if it's used for batch identification)
            modelBuilder.Entity<WarehouseProduct>()
                .HasKey(wp => new { wp.WarehouseId, wp.ProductId, wp.SupplierId, wp.ProductionDate });

            // Composite Key for ProductUnits (Fix for the error)
            modelBuilder.Entity<ProductUnits>()
                .HasKey(pu => new { pu.ProductId, pu.UnitId });

            // Composite Key for ProductTransferItem (for tracking items within a transfer)
            modelBuilder.Entity<ProductTransferItem>()
                .HasKey(pti => new { pti.ProductTransferId, pti.ProductId, pti.SupplierId, pti.ProductionDate });


            // --- 2. Define Unique Indexes ---
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Code)
                .IsUnique();

            modelBuilder.Entity<ReleaseOrder>()
                .HasIndex(ro => ro.OrderNumber)
                .IsUnique();

            modelBuilder.Entity<SupplyOrder>()
                .HasIndex(so => so.OrderNumber)
                .IsUnique();

            modelBuilder.Entity<ProductTransfer>() // TransferNumber for the header
                .HasIndex(pt => pt.TransferNumber)
                .IsUnique();


            // --- 3. Define ValueGeneratedNever for DateTime properties in composite keys ---
            // These ensure EF Core doesn't try to make ProductionDate an IDENTITY column.
            modelBuilder.Entity<WarehouseProduct>()
                .Property(e => e.ProductionDate)
                .ValueGeneratedNever();

            modelBuilder.Entity<SupplyOrderItem>()
                .Property(e => e.ProductionDate)
                .ValueGeneratedNever();

            modelBuilder.Entity<ProductTransferItem>() // For the ProductTransferItem entity
                .Property(e => e.ProductionDate)
                .ValueGeneratedNever();


            // --- 4. Define Relationships (Foreign Keys) ---

            // ProductTransfer (Header) relationships
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

            // ProductTransferItem (Detail) relationships
            modelBuilder.Entity<ProductTransferItem>()
                .HasOne(pti => pti.ProductTransfer) // ProductTransferItem belongs to a ProductTransfer
                .WithMany(pt => pt.Items) // ProductTransfer has many ProductTransferItems (requires ICollection<ProductTransferItem> Items on ProductTransfer)
                .HasForeignKey(pti => pti.ProductTransferId)
                .OnDelete(DeleteBehavior.Cascade); // Delete items if parent transfer is deleted

            modelBuilder.Entity<ProductTransferItem>()
                .HasOne(pti => pti.Product)
                .WithMany() // Product doesn't need a direct collection to ProductTransferItems if not used
                .HasForeignKey(pti => pti.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProductTransferItem>()
                .HasOne(pti => pti.Supplier)
                .WithMany() // Supplier doesn't need a direct collection to ProductTransferItems
                .HasForeignKey(pti => pti.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            // ProductUnits Relationships
            modelBuilder.Entity<ProductUnits>()
                .HasOne(pu => pu.Product)
                .WithMany(p => p.ProductUnits) // Assumes Product has ICollection<ProductUnits>
                .HasForeignKey(pu => pu.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // Adjust as needed

            modelBuilder.Entity<ProductUnits>()
                .HasOne(pu => pu.Unit)
                .WithMany(u => u.ProductUnits) // Assumes Unit has ICollection<ProductUnits>
                .HasForeignKey(pu => pu.UnitId)
                .OnDelete(DeleteBehavior.Cascade); // Adjust as needed

            // ReleaseOrder Relationships (ensure correct WithMany)
            modelBuilder.Entity<ReleaseOrder>()
                .HasOne(ro => ro.Warehouse)
                .WithMany(w => w.ReleaseOrders)
                .HasForeignKey(ro => ro.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReleaseOrder>()
                .HasOne(ro => ro.Supplier)
                .WithMany() // Assuming Supplier does not have ICollection<ReleaseOrder>
                .HasForeignKey(ro => ro.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReleaseOrderItem>()
                .HasOne(roi => roi.ReleaseOrder)
                .WithMany(ro => ro.Items)
                .HasForeignKey(roi => roi.ReleaseOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // SupplyOrder Relationships (ensure correct WithMany)
            modelBuilder.Entity<SupplyOrder>()
                .HasOne(so => so.Warehouse)
                .WithMany(w => w.SupplyOrders)
                .HasForeignKey(so => so.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SupplyOrder>()
                .HasOne(so => so.Supplier)
                .WithMany(s => s.SupplyOrders) // Assuming Supplier has ICollection<SupplyOrder>
                .HasForeignKey(so => so.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SupplyOrderItem>()
                .HasOne(soi => soi.SupplyOrder)
                .WithMany(so => so.Items)
                .HasForeignKey(soi => soi.SupplyOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // WarehouseProduct Relationships
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
                .HasOne(w => w.Manager)           // A Warehouse has one Manager
                .WithMany()                       // A Manager can have many Warehouses (no direct navigation property on Manager side needed if only one direction is used)
                .HasForeignKey(w => w.ManagerId);
        }
    }
}