using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BoomaEcommerce.Data.Migrations
{
    public partial class ProductPolicy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Policies_Products_MaxProductAmountPolicy_ProductGuid",
                table: "Policies");

            migrationBuilder.DropForeignKey(
                name: "FK_Policies_Products_MinProductAmountPolicy_ProductGuid",
                table: "Policies");

            migrationBuilder.DropIndex(
                name: "IX_Policies_MaxProductAmountPolicy_ProductGuid",
                table: "Policies");

            migrationBuilder.DropIndex(
                name: "IX_Policies_MinProductAmountPolicy_ProductGuid",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "MaxProductAmountPolicy_ProductGuid",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "MinProductAmountPolicy_ProductGuid",
                table: "Policies");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MaxProductAmountPolicy_ProductGuid",
                table: "Policies",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MinProductAmountPolicy_ProductGuid",
                table: "Policies",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Policies_MaxProductAmountPolicy_ProductGuid",
                table: "Policies",
                column: "MaxProductAmountPolicy_ProductGuid");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_MinProductAmountPolicy_ProductGuid",
                table: "Policies",
                column: "MinProductAmountPolicy_ProductGuid");

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
        }
    }
}
