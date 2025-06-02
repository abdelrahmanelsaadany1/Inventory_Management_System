using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class again : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductTransferItem_ProductTransfers_ProductTransferId",
                table: "ProductTransferItem");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductTransferItem_Products_ProductId",
                table: "ProductTransferItem");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductTransferItem_Suppliers_SupplierId",
                table: "ProductTransferItem");

            migrationBuilder.DropForeignKey(
                name: "FK_ReleaseOrders_Suppliers_SupplierId",
                table: "ReleaseOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_ReleaseOrders_Warehouses_WarehouseId",
                table: "ReleaseOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplyOrders_Suppliers_SupplierId",
                table: "SupplyOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplyOrders_Warehouses_WarehouseId",
                table: "SupplyOrders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WarehouseProducts",
                table: "WarehouseProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductTransferItem",
                table: "ProductTransferItem");

            migrationBuilder.DropIndex(
                name: "IX_ProductTransferItem_ProductTransferId",
                table: "ProductTransferItem");

            // Drop the Id column completely
            migrationBuilder.DropColumn(
                name: "Id",
                table: "ProductTransferItem");

            migrationBuilder.RenameTable(
                name: "ProductTransferItem",
                newName: "ProductTransferItems");

            migrationBuilder.RenameIndex(
                name: "IX_ProductTransferItem_SupplierId",
                table: "ProductTransferItems",
                newName: "IX_ProductTransferItems_SupplierId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductTransferItem_ProductId",
                table: "ProductTransferItems",
                newName: "IX_ProductTransferItems_ProductId");

            // Add the Id column back without IDENTITY
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ProductTransferItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_WarehouseProducts",
                table: "WarehouseProducts",
                columns: new[] { "WarehouseId", "ProductId", "SupplierId", "ProductionDate" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductTransferItems",
                table: "ProductTransferItems",
                columns: new[] { "ProductTransferId", "ProductId", "SupplierId", "ProductionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductTransfers_TransferNumber",
                table: "ProductTransfers",
                column: "TransferNumber",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTransferItems_ProductTransfers_ProductTransferId",
                table: "ProductTransferItems",
                column: "ProductTransferId",
                principalTable: "ProductTransfers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTransferItems_Products_ProductId",
                table: "ProductTransferItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTransferItems_Suppliers_SupplierId",
                table: "ProductTransferItems",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReleaseOrders_Suppliers_SupplierId",
                table: "ReleaseOrders",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReleaseOrders_Warehouses_WarehouseId",
                table: "ReleaseOrders",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SupplyOrders_Suppliers_SupplierId",
                table: "SupplyOrders",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SupplyOrders_Warehouses_WarehouseId",
                table: "SupplyOrders",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductTransferItems_ProductTransfers_ProductTransferId",
                table: "ProductTransferItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductTransferItems_Products_ProductId",
                table: "ProductTransferItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductTransferItems_Suppliers_SupplierId",
                table: "ProductTransferItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ReleaseOrders_Suppliers_SupplierId",
                table: "ReleaseOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_ReleaseOrders_Warehouses_WarehouseId",
                table: "ReleaseOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplyOrders_Suppliers_SupplierId",
                table: "SupplyOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplyOrders_Warehouses_WarehouseId",
                table: "SupplyOrders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WarehouseProducts",
                table: "WarehouseProducts");

            migrationBuilder.DropIndex(
                name: "IX_ProductTransfers_TransferNumber",
                table: "ProductTransfers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductTransferItems",
                table: "ProductTransferItems");

            // Drop the Id column
            migrationBuilder.DropColumn(
                name: "Id",
                table: "ProductTransferItems");

            migrationBuilder.RenameTable(
                name: "ProductTransferItems",
                newName: "ProductTransferItem");

            migrationBuilder.RenameIndex(
                name: "IX_ProductTransferItems_SupplierId",
                table: "ProductTransferItem",
                newName: "IX_ProductTransferItem_SupplierId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductTransferItems_ProductId",
                table: "ProductTransferItem",
                newName: "IX_ProductTransferItem_ProductId");

            // Add back the Id column with IDENTITY
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ProductTransferItem",
                type: "int",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WarehouseProducts",
                table: "WarehouseProducts",
                columns: new[] { "WarehouseId", "ProductId", "SupplierId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductTransferItem",
                table: "ProductTransferItem",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTransferItem_ProductTransferId",
                table: "ProductTransferItem",
                column: "ProductTransferId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTransferItem_ProductTransfers_ProductTransferId",
                table: "ProductTransferItem",
                column: "ProductTransferId",
                principalTable: "ProductTransfers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTransferItem_Products_ProductId",
                table: "ProductTransferItem",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTransferItem_Suppliers_SupplierId",
                table: "ProductTransferItem",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReleaseOrders_Suppliers_SupplierId",
                table: "ReleaseOrders",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReleaseOrders_Warehouses_WarehouseId",
                table: "ReleaseOrders",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SupplyOrders_Suppliers_SupplierId",
                table: "SupplyOrders",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SupplyOrders_Warehouses_WarehouseId",
                table: "SupplyOrders",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}