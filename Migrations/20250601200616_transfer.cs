using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class transfer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductTransfers_Products_ProductId",
                table: "ProductTransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductTransfers_Suppliers_SupplierId",
                table: "ProductTransfers");

            migrationBuilder.DropColumn(
                name: "ExpiryPeriodInDays",
                table: "ProductTransfers");

            migrationBuilder.DropColumn(
                name: "ProductionDate",
                table: "ProductTransfers");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "ProductTransfers");

            migrationBuilder.AlterColumn<int>(
                name: "SupplierId",
                table: "ProductTransfers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "ProductTransfers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "TransferNumber",
                table: "ProductTransfers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ProductTransferItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProductTransferId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    ProductionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiryPeriodInDays = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTransferItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductTransferItem_ProductTransfers_ProductTransferId",
                        column: x => x.ProductTransferId,
                        principalTable: "ProductTransfers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductTransferItem_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductTransferItem_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductTransferItem_ProductId",
                table: "ProductTransferItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTransferItem_ProductTransferId",
                table: "ProductTransferItem",
                column: "ProductTransferId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTransferItem_SupplierId",
                table: "ProductTransferItem",
                column: "SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTransfers_Products_ProductId",
                table: "ProductTransfers",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTransfers_Suppliers_SupplierId",
                table: "ProductTransfers",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductTransfers_Products_ProductId",
                table: "ProductTransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductTransfers_Suppliers_SupplierId",
                table: "ProductTransfers");

            migrationBuilder.DropTable(
                name: "ProductTransferItem");

            migrationBuilder.DropColumn(
                name: "TransferNumber",
                table: "ProductTransfers");

            migrationBuilder.AlterColumn<int>(
                name: "SupplierId",
                table: "ProductTransfers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "ProductTransfers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExpiryPeriodInDays",
                table: "ProductTransfers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProductionDate",
                table: "ProductTransfers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "ProductTransfers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTransfers_Products_ProductId",
                table: "ProductTransfers",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTransfers_Suppliers_SupplierId",
                table: "ProductTransfers",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
