using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BoomaEcommerce.Data.Migrations
{
    public partial class OfferWithoutAprrovers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductOffers",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    State = table.Column<int>(type: "int", nullable: false),
                    OfferPrice = table.Column<decimal>(type: "decimal(10,5)", precision: 10, scale: 5, nullable: false),
                    CounterOfferPrice = table.Column<decimal>(type: "decimal(10,5)", precision: 10, scale: 5, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductOffers", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_ProductOffers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductOffers_Products_ProductGuid",
                        column: x => x.ProductGuid,
                        principalTable: "Products",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductOffers_ProductGuid",
                table: "ProductOffers",
                column: "ProductGuid");

            migrationBuilder.CreateIndex(
                name: "IX_ProductOffers_UserId",
                table: "ProductOffers",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductOffers");
        }
    }
}
