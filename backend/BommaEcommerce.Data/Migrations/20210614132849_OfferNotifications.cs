using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BoomaEcommerce.Data.Migrations
{
    public partial class OfferNotifications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OfferApprovedNotification_OfferGuid",
                table: "Notifications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OfferDeclinedNotification_OfferGuid",
                table: "Notifications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OfferGuid",
                table: "Notifications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_OfferApprovedNotification_OfferGuid",
                table: "Notifications",
                column: "OfferApprovedNotification_OfferGuid");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_OfferDeclinedNotification_OfferGuid",
                table: "Notifications",
                column: "OfferDeclinedNotification_OfferGuid");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_OfferGuid",
                table: "Notifications",
                column: "OfferGuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_ProductOffers_OfferApprovedNotification_OfferGuid",
                table: "Notifications",
                column: "OfferApprovedNotification_OfferGuid",
                principalTable: "ProductOffers",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_ProductOffers_OfferDeclinedNotification_OfferGuid",
                table: "Notifications",
                column: "OfferDeclinedNotification_OfferGuid",
                principalTable: "ProductOffers",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_ProductOffers_OfferGuid",
                table: "Notifications",
                column: "OfferGuid",
                principalTable: "ProductOffers",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_ProductOffers_OfferApprovedNotification_OfferGuid",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_ProductOffers_OfferDeclinedNotification_OfferGuid",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_ProductOffers_OfferGuid",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_OfferApprovedNotification_OfferGuid",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_OfferDeclinedNotification_OfferGuid",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_OfferGuid",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "OfferApprovedNotification_OfferGuid",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "OfferDeclinedNotification_OfferGuid",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "OfferGuid",
                table: "Notifications");

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Invalidated = table.Column<bool>(type: "bit", nullable: false),
                    JwtId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Used = table.Column<bool>(type: "bit", nullable: false)
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
        }
    }
}
