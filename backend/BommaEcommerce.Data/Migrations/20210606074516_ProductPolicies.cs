using System;
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

            migrationBuilder.DropIndex(
                name: "IX_Policies_FirstPolicyGuid",
                table: "Policies");

            migrationBuilder.DropIndex(
                name: "IX_Policies_MaxProductAmountPolicy_ProductGuid",
                table: "Policies");

            migrationBuilder.DropIndex(
                name: "IX_Policies_MinProductAmountPolicy_ProductGuid",
                table: "Policies");

            migrationBuilder.DropIndex(
                name: "IX_Policies_SecondPolicyGuid",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "MaxProductAmountPolicy_ProductGuid",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "MinProductAmountPolicy_ProductGuid",
                table: "Policies");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_FirstPolicyGuid",
                table: "Policies",
                column: "FirstPolicyGuid",
                unique: true,
                filter: "[FirstPolicyGuid] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_SecondPolicyGuid",
                table: "Policies",
                column: "SecondPolicyGuid",
                unique: true,
                filter: "[SecondPolicyGuid] IS NOT NULL");

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
                name: "FK_Policies_Products_ProductGuid",
                table: "Policies");

            migrationBuilder.DropIndex(
                name: "IX_Policies_FirstPolicyGuid",
                table: "Policies");

            migrationBuilder.DropIndex(
                name: "IX_Policies_SecondPolicyGuid",
                table: "Policies");

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
                name: "IX_Policies_FirstPolicyGuid",
                table: "Policies",
                column: "FirstPolicyGuid");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_MaxProductAmountPolicy_ProductGuid",
                table: "Policies",
                column: "MaxProductAmountPolicy_ProductGuid");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_MinProductAmountPolicy_ProductGuid",
                table: "Policies",
                column: "MinProductAmountPolicy_ProductGuid");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_SecondPolicyGuid",
                table: "Policies",
                column: "SecondPolicyGuid");

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
