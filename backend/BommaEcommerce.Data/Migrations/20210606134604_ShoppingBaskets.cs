using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BoomaEcommerce.Data.Migrations
{
    public partial class ShoppingBaskets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShoppingCarts",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCarts", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_ShoppingCarts_AspNetUsers_Guid",
                        column: x => x.Guid,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoreOwnerships",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StoreGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreOwnerships", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_StoreOwnerships_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoreOwnerships_Stores_StoreGuid",
                        column: x => x.StoreGuid,
                        principalTable: "Stores",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShoppingBaskets",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StoreGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ShoppingCartGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingBaskets", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_ShoppingBaskets_ShoppingCarts_ShoppingCartGuid",
                        column: x => x.ShoppingCartGuid,
                        principalTable: "ShoppingCarts",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShoppingBaskets_Stores_StoreGuid",
                        column: x => x.StoreGuid,
                        principalTable: "Stores",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoreManagements",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StoreGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StoreOwnershipGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreManagements", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_StoreManagements_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoreManagements_StoreOwnerships_StoreOwnershipGuid",
                        column: x => x.StoreOwnershipGuid,
                        principalTable: "StoreOwnerships",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StoreManagements_Stores_StoreGuid",
                        column: x => x.StoreGuid,
                        principalTable: "Stores",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShoppingBasketPurchaseProducts",
                columns: table => new
                {
                    ShoppingBasketGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingBasketPurchaseProducts", x => new { x.ShoppingBasketGuid, x.Id });
                    table.ForeignKey(
                        name: "FK_ShoppingBasketPurchaseProducts_Products_ProductGuid",
                        column: x => x.ProductGuid,
                        principalTable: "Products",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShoppingBasketPurchaseProducts_ShoppingBaskets_ShoppingBasketGuid",
                        column: x => x.ShoppingBasketGuid,
                        principalTable: "ShoppingBaskets",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingBasketPurchaseProducts_ProductGuid",
                table: "ShoppingBasketPurchaseProducts",
                column: "ProductGuid");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingBaskets_ShoppingCartGuid",
                table: "ShoppingBaskets",
                column: "ShoppingCartGuid");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingBaskets_StoreGuid",
                table: "ShoppingBaskets",
                column: "StoreGuid");

            migrationBuilder.CreateIndex(
                name: "IX_StoreManagements_StoreGuid",
                table: "StoreManagements",
                column: "StoreGuid");

            migrationBuilder.CreateIndex(
                name: "IX_StoreManagements_StoreOwnershipGuid",
                table: "StoreManagements",
                column: "StoreOwnershipGuid");

            migrationBuilder.CreateIndex(
                name: "IX_StoreManagements_UserId",
                table: "StoreManagements",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreOwnerships_StoreGuid",
                table: "StoreOwnerships",
                column: "StoreGuid");

            migrationBuilder.CreateIndex(
                name: "IX_StoreOwnerships_UserId",
                table: "StoreOwnerships",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShoppingBasketPurchaseProducts");

            migrationBuilder.DropTable(
                name: "StoreManagements");

            migrationBuilder.DropTable(
                name: "ShoppingBaskets");

            migrationBuilder.DropTable(
                name: "StoreOwnerships");

            migrationBuilder.DropTable(
                name: "ShoppingCarts");
        }
    }
}
