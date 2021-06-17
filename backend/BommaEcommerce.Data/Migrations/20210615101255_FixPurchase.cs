using Microsoft.EntityFrameworkCore.Migrations;

namespace BoomaEcommerce.Data.Migrations
{
    public partial class FixPurchase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Purchase_AspNetUsers_BuyerId",
                table: "Purchase");

            migrationBuilder.DropForeignKey(
                name: "FK_StorePurchases_Purchase_PurchaseGuid",
                table: "StorePurchases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Purchase",
                table: "Purchase");

            migrationBuilder.RenameTable(
                name: "Purchase",
                newName: "Purchases");

            migrationBuilder.RenameIndex(
                name: "IX_Purchase_BuyerId",
                table: "Purchases",
                newName: "IX_Purchases_BuyerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Purchases",
                table: "Purchases",
                column: "Guid");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_AspNetUsers_BuyerId",
                table: "Purchases",
                column: "BuyerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StorePurchases_Purchases_PurchaseGuid",
                table: "StorePurchases",
                column: "PurchaseGuid",
                principalTable: "Purchases",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_AspNetUsers_BuyerId",
                table: "Purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_StorePurchases_Purchases_PurchaseGuid",
                table: "StorePurchases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Purchases",
                table: "Purchases");

            migrationBuilder.RenameTable(
                name: "Purchases",
                newName: "Purchase");

            migrationBuilder.RenameIndex(
                name: "IX_Purchases_BuyerId",
                table: "Purchase",
                newName: "IX_Purchase_BuyerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Purchase",
                table: "Purchase",
                column: "Guid");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchase_AspNetUsers_BuyerId",
                table: "Purchase",
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
        }
    }
}
