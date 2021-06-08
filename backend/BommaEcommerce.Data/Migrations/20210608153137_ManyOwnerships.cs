using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BoomaEcommerce.Data.Migrations
{
    public partial class ManyOwnerships : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "StoreOwnershipGuid",
                table: "StoreOwnerships",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoreOwnerships_StoreOwnershipGuid",
                table: "StoreOwnerships",
                column: "StoreOwnershipGuid");

            migrationBuilder.AddForeignKey(
                name: "FK_StoreOwnerships_StoreOwnerships_StoreOwnershipGuid",
                table: "StoreOwnerships",
                column: "StoreOwnershipGuid",
                principalTable: "StoreOwnerships",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoreOwnerships_StoreOwnerships_StoreOwnershipGuid",
                table: "StoreOwnerships");

            migrationBuilder.DropIndex(
                name: "IX_StoreOwnerships_StoreOwnershipGuid",
                table: "StoreOwnerships");

            migrationBuilder.DropColumn(
                name: "StoreOwnershipGuid",
                table: "StoreOwnerships");
        }
    }
}
