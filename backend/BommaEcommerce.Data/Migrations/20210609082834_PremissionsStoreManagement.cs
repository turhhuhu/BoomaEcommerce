using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BoomaEcommerce.Data.Migrations
{
    public partial class PremissionsStoreManagement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
