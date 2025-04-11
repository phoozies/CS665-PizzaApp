using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CS665_PizzaRestaurantApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedOrderModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "OrderModels");

            migrationBuilder.AddColumn<int>(
                name: "MenuItemModelItemID",
                table: "OrderDetailModels",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderModelOrderID",
                table: "OrderDetailModels",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetailModels_MenuItemModelItemID",
                table: "OrderDetailModels",
                column: "MenuItemModelItemID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetailModels_OrderModelOrderID",
                table: "OrderDetailModels",
                column: "OrderModelOrderID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetailModels_MenuItemModels_MenuItemModelItemID",
                table: "OrderDetailModels",
                column: "MenuItemModelItemID",
                principalTable: "MenuItemModels",
                principalColumn: "ItemID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetailModels_OrderModels_OrderModelOrderID",
                table: "OrderDetailModels",
                column: "OrderModelOrderID",
                principalTable: "OrderModels",
                principalColumn: "OrderID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetailModels_MenuItemModels_MenuItemModelItemID",
                table: "OrderDetailModels");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetailModels_OrderModels_OrderModelOrderID",
                table: "OrderDetailModels");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetailModels_MenuItemModelItemID",
                table: "OrderDetailModels");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetailModels_OrderModelOrderID",
                table: "OrderDetailModels");

            migrationBuilder.DropColumn(
                name: "MenuItemModelItemID",
                table: "OrderDetailModels");

            migrationBuilder.DropColumn(
                name: "OrderModelOrderID",
                table: "OrderDetailModels");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "OrderModels",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
