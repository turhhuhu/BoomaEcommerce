using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BoomaEcommerce.Data.Migrations
{
    public partial class DiscountTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "StoreDiscountGuid",
                table: "Stores",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountedPrice",
                table: "StorePurchasePurchaseProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountedPrice",
                table: "StorePurchase",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountedPrice",
                table: "ShoppingBasketPurchaseProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountedPrice",
                table: "Purchase",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "CanCreateDiscounts",
                table: "Permissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanDeleteDiscount",
                table: "Permissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanGetDiscountInfo",
                table: "Permissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Discounts",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Percentage = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PolicyGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompositeDiscountGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DiscountType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discounts", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_Discounts_Discounts_CompositeDiscountGuid",
                        column: x => x.CompositeDiscountGuid,
                        principalTable: "Discounts",
                        principalColumn: "Guid");
                    table.ForeignKey(
                        name: "FK_Discounts_Policies_PolicyGuid",
                        column: x => x.PolicyGuid,
                        principalTable: "Policies",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Discounts_Products_ProductGuid",
                        column: x => x.ProductGuid,
                        principalTable: "Products",
                        principalColumn: "Guid");
                });

            migrationBuilder.CreateTable(
                name: "DiscountOperators",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiscountOperatorType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountOperators", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_DiscountOperators_Discounts_Guid",
                        column: x => x.Guid,
                        principalTable: "Discounts",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stores_StoreDiscountGuid",
                table: "Stores",
                column: "StoreDiscountGuid");

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_CompositeDiscountGuid",
                table: "Discounts",
                column: "CompositeDiscountGuid");

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_PolicyGuid",
                table: "Discounts",
                column: "PolicyGuid");

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_ProductGuid",
                table: "Discounts",
                column: "ProductGuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_Discounts_StoreDiscountGuid",
                table: "Stores",
                column: "StoreDiscountGuid",
                principalTable: "Discounts",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stores_Discounts_StoreDiscountGuid",
                table: "Stores");

            migrationBuilder.DropTable(
                name: "DiscountOperators");

            migrationBuilder.DropTable(
                name: "Discounts");

            migrationBuilder.DropIndex(
                name: "IX_Stores_StoreDiscountGuid",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "StoreDiscountGuid",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "DiscountedPrice",
                table: "StorePurchasePurchaseProducts");

            migrationBuilder.DropColumn(
                name: "DiscountedPrice",
                table: "StorePurchase");

            migrationBuilder.DropColumn(
                name: "DiscountedPrice",
                table: "ShoppingBasketPurchaseProducts");

            migrationBuilder.DropColumn(
                name: "DiscountedPrice",
                table: "Purchase");

            migrationBuilder.DropColumn(
                name: "CanCreateDiscounts",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "CanDeleteDiscount",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "CanGetDiscountInfo",
                table: "Permissions");
        }
    }
}
