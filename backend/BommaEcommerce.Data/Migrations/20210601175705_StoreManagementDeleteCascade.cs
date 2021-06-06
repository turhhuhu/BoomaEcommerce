using Microsoft.EntityFrameworkCore.Migrations;

namespace BoomaEcommerce.Data.Migrations
{
    public partial class StoreManagementDeleteCascade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoreManagements_AspNetUsers_UserId",
                table: "StoreManagements");

            migrationBuilder.DropForeignKey(
                name: "FK_StoreManagements_Stores_StoreGuid",
                table: "StoreManagements");

            migrationBuilder.AddForeignKey(
                name: "FK_StoreManagements_AspNetUsers_UserId",
                table: "StoreManagements",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StoreManagements_Stores_StoreGuid",
                table: "StoreManagements",
                column: "StoreGuid",
                principalTable: "Stores",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoreManagements_AspNetUsers_UserId",
                table: "StoreManagements");

            migrationBuilder.DropForeignKey(
                name: "FK_StoreManagements_Stores_StoreGuid",
                table: "StoreManagements");

            migrationBuilder.AddForeignKey(
                name: "FK_StoreManagements_AspNetUsers_UserId",
                table: "StoreManagements",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoreManagements_Stores_StoreGuid",
                table: "StoreManagements",
                column: "StoreGuid",
                principalTable: "Stores",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
