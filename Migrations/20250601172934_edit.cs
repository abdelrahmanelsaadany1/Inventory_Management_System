using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class edit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReleaseOrders_Customers_CustomerId",
                table: "ReleaseOrders");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "ReleaseOrders",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "SupplierId",
                table: "ReleaseOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ReleaseOrders_SupplierId",
                table: "ReleaseOrders",
                column: "SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReleaseOrders_Customers_CustomerId",
                table: "ReleaseOrders",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReleaseOrders_Suppliers_SupplierId",
                table: "ReleaseOrders",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReleaseOrders_Customers_CustomerId",
                table: "ReleaseOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_ReleaseOrders_Suppliers_SupplierId",
                table: "ReleaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_ReleaseOrders_SupplierId",
                table: "ReleaseOrders");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "ReleaseOrders");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "ReleaseOrders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ReleaseOrders_Customers_CustomerId",
                table: "ReleaseOrders",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
