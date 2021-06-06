using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BoomaEcommerce.Data.Migrations
{
    public partial class AddStoreOwnerShip : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "StoreOwnershipGuid",
                table: "StoreManagements",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StoreOwnerships",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StoreGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreOwnerships", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_StoreOwnerships_Stores_StoreGuid",
                        column: x => x.StoreGuid,
                        principalTable: "Stores",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StoreManagements_StoreOwnershipGuid",
                table: "StoreManagements",
                column: "StoreOwnershipGuid");

            migrationBuilder.CreateIndex(
                name: "IX_StoreOwnerships_StoreGuid",
                table: "StoreOwnerships",
                column: "StoreGuid");

            migrationBuilder.AddForeignKey(
                name: "FK_StoreManagements_StoreOwnerships_StoreOwnershipGuid",
                table: "StoreManagements",
                column: "StoreOwnershipGuid",
                principalTable: "StoreOwnerships",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoreManagements_StoreOwnerships_StoreOwnershipGuid",
                table: "StoreManagements");

            migrationBuilder.DropTable(
                name: "StoreOwnerships");

            migrationBuilder.DropIndex(
                name: "IX_StoreManagements_StoreOwnershipGuid",
                table: "StoreManagements");

            migrationBuilder.DropColumn(
                name: "StoreOwnershipGuid",
                table: "StoreManagements");
        }
    }
}
