using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BoomaEcommerce.Data.Migrations
{
    public partial class StoreWithFounder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "StoreFounderId",
                table: "Stores",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stores_StoreFounderId",
                table: "Stores",
                column: "StoreFounderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_AspNetUsers_StoreFounderId",
                table: "Stores",
                column: "StoreFounderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stores_AspNetUsers_StoreFounderId",
                table: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_Stores_StoreFounderId",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "StoreFounderId",
                table: "Stores");
        }
    }
}
