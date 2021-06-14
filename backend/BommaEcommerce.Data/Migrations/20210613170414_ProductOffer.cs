using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BoomaEcommerce.Data.Migrations
{
    public partial class ProductOffer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProductOfferGuid",
                table: "ApproversOffers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApproversOffers_ProductOfferGuid",
                table: "ApproversOffers",
                column: "ProductOfferGuid");

            migrationBuilder.AddForeignKey(
                name: "FK_ApproversOffers_ProductOffers_ProductOfferGuid",
                table: "ApproversOffers",
                column: "ProductOfferGuid",
                principalTable: "ProductOffers",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApproversOffers_ProductOffers_ProductOfferGuid",
                table: "ApproversOffers");

            migrationBuilder.DropIndex(
                name: "IX_ApproversOffers_ProductOfferGuid",
                table: "ApproversOffers");

            migrationBuilder.DropColumn(
                name: "ProductOfferGuid",
                table: "ApproversOffers");
        }
    }
}
