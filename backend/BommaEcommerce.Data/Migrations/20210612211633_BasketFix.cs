using Microsoft.EntityFrameworkCore.Migrations;

namespace BoomaEcommerce.Data.Migrations
{
    public partial class BasketFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Discounts",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Discounts");
        }
    }
}
