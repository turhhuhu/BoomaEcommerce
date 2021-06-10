using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BoomaEcommerce.Data.Migrations
{
    public partial class StorePurchases : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StorePurchase_AspNetUsers_BuyerId",
                table: "StorePurchase");

            migrationBuilder.DropForeignKey(
                name: "FK_StorePurchase_Purchase_PurchaseGuid",
                table: "StorePurchase");

            migrationBuilder.DropForeignKey(
                name: "FK_StorePurchase_Stores_StoreGuid",
                table: "StorePurchase");

            migrationBuilder.DropForeignKey(
                name: "FK_StorePurchasePurchaseProducts_StorePurchase_StorePurchaseGuid",
                table: "StorePurchasePurchaseProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StorePurchase",
                table: "StorePurchase");

            migrationBuilder.RenameTable(
                name: "StorePurchase",
                newName: "StorePurchases");

            migrationBuilder.RenameIndex(
                name: "IX_StorePurchase_StoreGuid",
                table: "StorePurchases",
                newName: "IX_StorePurchases_StoreGuid");

            migrationBuilder.RenameIndex(
                name: "IX_StorePurchase_PurchaseGuid",
                table: "StorePurchases",
                newName: "IX_StorePurchases_PurchaseGuid");

            migrationBuilder.RenameIndex(
                name: "IX_StorePurchase_BuyerId",
                table: "StorePurchases",
                newName: "IX_StorePurchases_BuyerId");

            migrationBuilder.AlterColumn<Guid>(
                name: "PurchaseGuid",
                table: "StorePurchases",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StorePurchases",
                table: "StorePurchases",
                column: "Guid");

            migrationBuilder.AddForeignKey(
                name: "FK_StorePurchasePurchaseProducts_StorePurchases_StorePurchaseGuid",
                table: "StorePurchasePurchaseProducts",
                column: "StorePurchaseGuid",
                principalTable: "StorePurchases",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StorePurchases_AspNetUsers_BuyerId",
                table: "StorePurchases",
                column: "BuyerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StorePurchases_Purchase_PurchaseGuid",
                table: "StorePurchases",
                column: "PurchaseGuid",
                principalTable: "Purchase",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StorePurchases_Stores_StoreGuid",
                table: "StorePurchases",
                column: "StoreGuid",
                principalTable: "Stores",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StorePurchasePurchaseProducts_StorePurchases_StorePurchaseGuid",
                table: "StorePurchasePurchaseProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_StorePurchases_AspNetUsers_BuyerId",
                table: "StorePurchases");

            migrationBuilder.DropForeignKey(
                name: "FK_StorePurchases_Purchase_PurchaseGuid",
                table: "StorePurchases");

            migrationBuilder.DropForeignKey(
                name: "FK_StorePurchases_Stores_StoreGuid",
                table: "StorePurchases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StorePurchases",
                table: "StorePurchases");

            migrationBuilder.RenameTable(
                name: "StorePurchases",
                newName: "StorePurchase");

            migrationBuilder.RenameIndex(
                name: "IX_StorePurchases_StoreGuid",
                table: "StorePurchase",
                newName: "IX_StorePurchase_StoreGuid");

            migrationBuilder.RenameIndex(
                name: "IX_StorePurchases_PurchaseGuid",
                table: "StorePurchase",
                newName: "IX_StorePurchase_PurchaseGuid");

            migrationBuilder.RenameIndex(
                name: "IX_StorePurchases_BuyerId",
                table: "StorePurchase",
                newName: "IX_StorePurchase_BuyerId");

            migrationBuilder.AlterColumn<Guid>(
                name: "PurchaseGuid",
                table: "StorePurchase",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StorePurchase",
                table: "StorePurchase",
                column: "Guid");

            migrationBuilder.AddForeignKey(
                name: "FK_StorePurchase_AspNetUsers_BuyerId",
                table: "StorePurchase",
                column: "BuyerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StorePurchase_Purchase_PurchaseGuid",
                table: "StorePurchase",
                column: "PurchaseGuid",
                principalTable: "Purchase",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StorePurchase_Stores_StoreGuid",
                table: "StorePurchase",
                column: "StoreGuid",
                principalTable: "Stores",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StorePurchasePurchaseProducts_StorePurchase_StorePurchaseGuid",
                table: "StorePurchasePurchaseProducts",
                column: "StorePurchaseGuid",
                principalTable: "StorePurchase",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
