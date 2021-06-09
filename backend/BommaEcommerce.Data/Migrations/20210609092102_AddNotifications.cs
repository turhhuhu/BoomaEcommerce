using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BoomaEcommerce.Data.Migrations
{
    public partial class AddNotifications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BuyerId",
                table: "Notifications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DismissingUserId",
                table: "Notifications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreGuid",
                table: "Notifications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StorePurchaseGuid",
                table: "Notifications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StorePurchaseNotification_StoreGuid",
                table: "Notifications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "notification_type",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_BuyerId",
                table: "Notifications",
                column: "BuyerId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_DismissingUserId",
                table: "Notifications",
                column: "DismissingUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_StoreGuid",
                table: "Notifications",
                column: "StoreGuid");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_StorePurchaseNotification_StoreGuid",
                table: "Notifications",
                column: "StorePurchaseNotification_StoreGuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetUsers_BuyerId",
                table: "Notifications",
                column: "BuyerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetUsers_DismissingUserId",
                table: "Notifications",
                column: "DismissingUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Stores_StoreGuid",
                table: "Notifications",
                column: "StoreGuid",
                principalTable: "Stores",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Stores_StorePurchaseNotification_StoreGuid",
                table: "Notifications",
                column: "StorePurchaseNotification_StoreGuid",
                principalTable: "Stores",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers_BuyerId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers_DismissingUserId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Stores_StoreGuid",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Stores_StorePurchaseNotification_StoreGuid",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_BuyerId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_DismissingUserId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_StoreGuid",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_StorePurchaseNotification_StoreGuid",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "BuyerId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "DismissingUserId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "StoreGuid",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "StorePurchaseGuid",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "StorePurchaseNotification_StoreGuid",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "notification_type",
                table: "Notifications");
        }
    }
}
