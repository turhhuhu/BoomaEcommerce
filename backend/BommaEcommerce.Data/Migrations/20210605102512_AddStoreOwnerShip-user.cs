using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BoomaEcommerce.Data.Migrations
{
    public partial class AddStoreOwnerShipuser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "StoreOwnerships",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoreOwnerships_UserId",
                table: "StoreOwnerships",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_StoreOwnerships_AspNetUsers_UserId",
                table: "StoreOwnerships",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoreOwnerships_AspNetUsers_UserId",
                table: "StoreOwnerships");

            migrationBuilder.DropIndex(
                name: "IX_StoreOwnerships_UserId",
                table: "StoreOwnerships");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "StoreOwnerships");
        }
    }
}
