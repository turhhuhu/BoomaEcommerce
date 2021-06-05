using Microsoft.EntityFrameworkCore.Migrations;

namespace BoomaEcommerce.Data.Migrations
{
    public partial class ProductPolicies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Policies_Products_MaxProductAmountPolicy_ProductGuid",
                table: "Policies");

            migrationBuilder.DropForeignKey(
                name: "FK_Policies_Products_MinProductAmountPolicy_ProductGuid",
                table: "Policies");

            migrationBuilder.DropForeignKey(
                name: "FK_Policies_Products_ProductGuid",
                table: "Policies");

            migrationBuilder.AddForeignKey(
                name: "FK_Policies_Products_MaxProductAmountPolicy_ProductGuid",
                table: "Policies",
                column: "MaxProductAmountPolicy_ProductGuid",
                principalTable: "Products",
                principalColumn: "Guid");

            migrationBuilder.AddForeignKey(
                name: "FK_Policies_Products_MinProductAmountPolicy_ProductGuid",
                table: "Policies",
                column: "MinProductAmountPolicy_ProductGuid",
                principalTable: "Products",
                principalColumn: "Guid");

            migrationBuilder.AddForeignKey(
                name: "FK_Policies_Products_ProductGuid",
                table: "Policies",
                column: "ProductGuid",
                principalTable: "Products",
                principalColumn: "Guid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Policies_Products_MaxProductAmountPolicy_ProductGuid",
                table: "Policies");

            migrationBuilder.DropForeignKey(
                name: "FK_Policies_Products_MinProductAmountPolicy_ProductGuid",
                table: "Policies");

            migrationBuilder.DropForeignKey(
                name: "FK_Policies_Products_ProductGuid",
                table: "Policies");

            migrationBuilder.AddForeignKey(
                name: "FK_Policies_Products_MaxProductAmountPolicy_ProductGuid",
                table: "Policies",
                column: "MaxProductAmountPolicy_ProductGuid",
                principalTable: "Products",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Policies_Products_MinProductAmountPolicy_ProductGuid",
                table: "Policies",
                column: "MinProductAmountPolicy_ProductGuid",
                principalTable: "Products",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Policies_Products_ProductGuid",
                table: "Policies",
                column: "ProductGuid",
                principalTable: "Products",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
