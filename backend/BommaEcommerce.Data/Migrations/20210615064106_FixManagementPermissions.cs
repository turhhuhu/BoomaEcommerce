using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BoomaEcommerce.Data.Migrations
{
    public partial class FixManagementPermissions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_StoreManagements_StoreManagementGuid",
                table: "Permissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Permissions",
                table: "Permissions");

            migrationBuilder.RenameTable(
                name: "Permissions",
                newName: "ManagementPermissions");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ManagementPermissions",
                table: "ManagementPermissions",
                column: "StoreManagementGuid");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JwtId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Used = table.Column<bool>(type: "bit", nullable: false),
                    Invalidated = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_AspNetUsers_Guid",
                        column: x => x.Guid,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_ManagementPermissions_StoreManagements_StoreManagementGuid",
                table: "ManagementPermissions",
                column: "StoreManagementGuid",
                principalTable: "StoreManagements",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ManagementPermissions_StoreManagements_StoreManagementGuid",
                table: "ManagementPermissions");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ManagementPermissions",
                table: "ManagementPermissions");

            migrationBuilder.RenameTable(
                name: "ManagementPermissions",
                newName: "Permissions");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Permissions",
                table: "Permissions",
                column: "StoreManagementGuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_StoreManagements_StoreManagementGuid",
                table: "Permissions",
                column: "StoreManagementGuid",
                principalTable: "StoreManagements",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
