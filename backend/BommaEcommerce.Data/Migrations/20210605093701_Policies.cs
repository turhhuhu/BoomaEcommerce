using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BoomaEcommerce.Data.Migrations
{
    public partial class Policies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_AspNetUsers_UserId",
                table: "Notification");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notification",
                table: "Notification");

            migrationBuilder.RenameTable(
                name: "Notification",
                newName: "Notifications");

            migrationBuilder.RenameIndex(
                name: "IX_Notification_UserId",
                table: "Notifications",
                newName: "IX_Notifications_UserId");

            migrationBuilder.AddColumn<Guid>(
                name: "StorePolicyGuid",
                table: "Stores",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications",
                column: "Guid");

            migrationBuilder.CreateTable(
                name: "Policies",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ErrorPrefix = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Level = table.Column<int>(type: "int", nullable: false),
                    CompositePolicyGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PolicyType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MinAge = table.Column<int>(type: "int", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaxAmount = table.Column<int>(type: "int", nullable: true),
                    MaxProductAmountPolicy_ProductGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MaxProductAmountPolicy_MaxAmount = table.Column<int>(type: "int", nullable: true),
                    MaxTotalAmountPolicy_MaxAmount = table.Column<int>(type: "int", nullable: true),
                    MinCategoryAmountPolicy_Category = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MinAmount = table.Column<int>(type: "int", nullable: true),
                    MinProductAmountPolicy_ProductGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MinProductAmountPolicy_MinAmount = table.Column<int>(type: "int", nullable: true),
                    MinTotalAmountPolicy_MinAmount = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Policies", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_Policies_Policies_CompositePolicyGuid",
                        column: x => x.CompositePolicyGuid,
                        principalTable: "Policies",
                        principalColumn: "Guid");
                    table.ForeignKey(
                        name: "FK_Policies_Products_MaxProductAmountPolicy_ProductGuid",
                        column: x => x.MaxProductAmountPolicy_ProductGuid,
                        principalTable: "Products",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Policies_Products_MinProductAmountPolicy_ProductGuid",
                        column: x => x.MinProductAmountPolicy_ProductGuid,
                        principalTable: "Products",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Policies_Products_ProductGuid",
                        column: x => x.ProductGuid,
                        principalTable: "Products",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PolicyOperators",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ErrorPrefix = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Level = table.Column<int>(type: "int", nullable: false),
                    PolicyOperatorType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolicyOperators", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_PolicyOperators_Policies_Guid",
                        column: x => x.Guid,
                        principalTable: "Policies",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stores_StorePolicyGuid",
                table: "Stores",
                column: "StorePolicyGuid");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_CompositePolicyGuid",
                table: "Policies",
                column: "CompositePolicyGuid");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_MaxProductAmountPolicy_ProductGuid",
                table: "Policies",
                column: "MaxProductAmountPolicy_ProductGuid");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_MinProductAmountPolicy_ProductGuid",
                table: "Policies",
                column: "MinProductAmountPolicy_ProductGuid");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_ProductGuid",
                table: "Policies",
                column: "ProductGuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetUsers_UserId",
                table: "Notifications",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_Policies_StorePolicyGuid",
                table: "Stores",
                column: "StorePolicyGuid",
                principalTable: "Policies",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers_UserId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Stores_Policies_StorePolicyGuid",
                table: "Stores");

            migrationBuilder.DropTable(
                name: "PolicyOperators");

            migrationBuilder.DropTable(
                name: "Policies");

            migrationBuilder.DropIndex(
                name: "IX_Stores_StorePolicyGuid",
                table: "Stores");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "StorePolicyGuid",
                table: "Stores");

            migrationBuilder.RenameTable(
                name: "Notifications",
                newName: "Notification");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_UserId",
                table: "Notification",
                newName: "IX_Notification_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notification",
                table: "Notification",
                column: "Guid");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_AspNetUsers_UserId",
                table: "Notification",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
