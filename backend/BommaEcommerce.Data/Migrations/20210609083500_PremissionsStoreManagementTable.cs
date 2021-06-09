using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BoomaEcommerce.Data.Migrations
{
    public partial class PremissionsStoreManagementTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Permissions_CanAddProduct",
                table: "StoreManagements");

            migrationBuilder.DropColumn(
                name: "Permissions_CanCreatePolicy",
                table: "StoreManagements");

            migrationBuilder.DropColumn(
                name: "Permissions_CanDeletePolicy",
                table: "StoreManagements");

            migrationBuilder.DropColumn(
                name: "Permissions_CanDeleteProduct",
                table: "StoreManagements");

            migrationBuilder.DropColumn(
                name: "Permissions_CanGetPolicyInfo",
                table: "StoreManagements");

            migrationBuilder.DropColumn(
                name: "Permissions_CanGetSellersInfo",
                table: "StoreManagements");

            migrationBuilder.DropColumn(
                name: "Permissions_CanUpdatePolicyInfo",
                table: "StoreManagements");

            migrationBuilder.DropColumn(
                name: "Permissions_CanUpdateProduct",
                table: "StoreManagements");

            migrationBuilder.DropColumn(
                name: "Permissions_Guid",
                table: "StoreManagements");

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    StoreManagementGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CanAddProduct = table.Column<bool>(type: "bit", nullable: false),
                    CanDeleteProduct = table.Column<bool>(type: "bit", nullable: false),
                    CanUpdateProduct = table.Column<bool>(type: "bit", nullable: false),
                    CanGetSellersInfo = table.Column<bool>(type: "bit", nullable: false),
                    CanCreatePolicy = table.Column<bool>(type: "bit", nullable: false),
                    CanDeletePolicy = table.Column<bool>(type: "bit", nullable: false),
                    CanGetPolicyInfo = table.Column<bool>(type: "bit", nullable: false),
                    CanUpdatePolicyInfo = table.Column<bool>(type: "bit", nullable: false),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.StoreManagementGuid);
                    table.ForeignKey(
                        name: "FK_Permissions_StoreManagements_StoreManagementGuid",
                        column: x => x.StoreManagementGuid,
                        principalTable: "StoreManagements",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.AddColumn<bool>(
                name: "Permissions_CanAddProduct",
                table: "StoreManagements",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Permissions_CanCreatePolicy",
                table: "StoreManagements",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Permissions_CanDeletePolicy",
                table: "StoreManagements",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Permissions_CanDeleteProduct",
                table: "StoreManagements",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Permissions_CanGetPolicyInfo",
                table: "StoreManagements",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Permissions_CanGetSellersInfo",
                table: "StoreManagements",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Permissions_CanUpdatePolicyInfo",
                table: "StoreManagements",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Permissions_CanUpdateProduct",
                table: "StoreManagements",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Permissions_Guid",
                table: "StoreManagements",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
