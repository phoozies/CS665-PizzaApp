using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CS665_PizzaRestaurantApp.Migrations
{
    /// <inheritdoc />
    public partial class MenuImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "MenuItemModels",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "MenuItemModels");
        }
    }
}
