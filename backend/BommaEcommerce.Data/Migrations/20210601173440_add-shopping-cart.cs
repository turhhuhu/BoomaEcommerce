using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BoomaEcommerce.Data.Migrations
{
    public partial class addshoppingcart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_storeManagements_AspNetUsers_UserId",
                table: "storeManagements");

            migrationBuilder.DropForeignKey(
                name: "FK_storeManagements_Stores_StoreGuid",
                table: "storeManagements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_storeManagements",
                table: "storeManagements");

            migrationBuilder.RenameTable(
                name: "storeManagements",
                newName: "StoreManagements");

            migrationBuilder.RenameIndex(
                name: "IX_storeManagements_UserId",
                table: "StoreManagements",
                newName: "IX_StoreManagements_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_storeManagements_StoreGuid",
                table: "StoreManagements",
                newName: "IX_StoreManagements_StoreGuid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StoreManagements",
                table: "StoreManagements",
                column: "Guid");

            migrationBuilder.CreateTable(
                name: "ShoppingCarts",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCarts", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_ShoppingCarts_AspNetUsers_Guid",
                        column: x => x.Guid,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoreManagements_AspNetUsers_UserId",
                table: "StoreManagements");

            migrationBuilder.DropForeignKey(
                name: "FK_StoreManagements_Stores_StoreGuid",
                table: "StoreManagements");

            migrationBuilder.DropTable(
                name: "ShoppingCarts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StoreManagements",
                table: "StoreManagements");

            migrationBuilder.RenameTable(
                name: "StoreManagements",
                newName: "storeManagements");

            migrationBuilder.RenameIndex(
                name: "IX_StoreManagements_UserId",
                table: "storeManagements",
                newName: "IX_storeManagements_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_StoreManagements_StoreGuid",
                table: "storeManagements",
                newName: "IX_storeManagements_StoreGuid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_storeManagements",
                table: "storeManagements",
                column: "Guid");

            migrationBuilder.AddForeignKey(
                name: "FK_storeManagements_AspNetUsers_UserId",
                table: "storeManagements",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_storeManagements_Stores_StoreGuid",
                table: "storeManagements",
                column: "StoreGuid",
                principalTable: "Stores",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
