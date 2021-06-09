using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BoomaEcommerce.Data.Migrations
{
    public partial class InitialDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WasSeen = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_Notifications_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Purchase",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BuyerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Purchase", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_Purchase_AspNetUsers_BuyerId",
                        column: x => x.BuyerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                });

            migrationBuilder.CreateTable(
                name: "StorePurchase",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BuyerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StoreGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TotalPrice = table.Column<decimal>(type: "decimal(10,5)", precision: 10, scale: 5, nullable: false),
                    PurchaseGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorePurchase", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_StorePurchase_AspNetUsers_BuyerId",
                        column: x => x.BuyerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StorePurchase_Purchase_PurchaseGuid",
                        column: x => x.PurchaseGuid,
                        principalTable: "Purchase",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Stores",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StoreName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StoreFounderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Rating = table.Column<float>(type: "real", nullable: false),
                    StorePolicyGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stores", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_Stores_AspNetUsers_StoreFounderId",
                        column: x => x.StoreFounderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StoreGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(10,5)", precision: 10, scale: 5, nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: false),
                    IsSoftDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_Products_Stores_StoreGuid",
                        column: x => x.StoreGuid,
                        principalTable: "Stores",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
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
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Policies",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ErrorPrefix = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Level = table.Column<int>(type: "int", nullable: false),
                    CompositePolicyGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FirstPolicyGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PolicyType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SecondPolicyGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaxAmount = table.Column<int>(type: "int", nullable: true),
                    MaxTotalAmountPolicy_MaxAmount = table.Column<int>(type: "int", nullable: true),
                    MinCategoryAmountPolicy_Category = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MinAmount = table.Column<int>(type: "int", nullable: true),
                    MinTotalAmountPolicy_MinAmount = table.Column<int>(type: "int", nullable: true),
                    ProductGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MinAge = table.Column<int>(type: "int", nullable: true),
                    MaxProductAmountPolicy_MaxAmount = table.Column<int>(type: "int", nullable: true),
                    MinProductAmountPolicy_MinAmount = table.Column<int>(type: "int", nullable: true)
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
                        name: "FK_Policies_Policies_FirstPolicyGuid",
                        column: x => x.FirstPolicyGuid,
                        principalTable: "Policies",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Policies_Policies_SecondPolicyGuid",
                        column: x => x.SecondPolicyGuid,
                        principalTable: "Policies",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Policies_Products_ProductGuid",
                        column: x => x.ProductGuid,
                        principalTable: "Products",
                        principalColumn: "Guid");
                });

            migrationBuilder.CreateTable(
                name: "StorePurchasePurchaseProducts",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,5)", precision: 10, scale: 5, nullable: false),
                    StorePurchaseGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorePurchasePurchaseProducts", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_StorePurchasePurchaseProducts_Products_ProductGuid",
                        column: x => x.ProductGuid,
                        principalTable: "Products",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StorePurchasePurchaseProducts_StorePurchase_StorePurchaseGuid",
                        column: x => x.StorePurchaseGuid,
                        principalTable: "StorePurchase",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShoppingBasketPurchaseProducts",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,5)", precision: 10, scale: 5, nullable: false),
                    ShoppingBasketGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingBasketPurchaseProducts", x => x.Guid);
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
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_CompositePolicyGuid",
                table: "Policies",
                column: "CompositePolicyGuid");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_FirstPolicyGuid",
                table: "Policies",
                column: "FirstPolicyGuid",
                unique: true,
                filter: "[FirstPolicyGuid] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_ProductGuid",
                table: "Policies",
                column: "ProductGuid");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_SecondPolicyGuid",
                table: "Policies",
                column: "SecondPolicyGuid",
                unique: true,
                filter: "[SecondPolicyGuid] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Products_StoreGuid",
                table: "Products",
                column: "StoreGuid");

            migrationBuilder.CreateIndex(
                name: "IX_Purchase_BuyerId",
                table: "Purchase",
                column: "BuyerId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingBasketPurchaseProducts_ProductGuid",
                table: "ShoppingBasketPurchaseProducts",
                column: "ProductGuid");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingBasketPurchaseProducts_ShoppingBasketGuid",
                table: "ShoppingBasketPurchaseProducts",
                column: "ShoppingBasketGuid");

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

            migrationBuilder.CreateIndex(
                name: "IX_StorePurchase_BuyerId",
                table: "StorePurchase",
                column: "BuyerId");

            migrationBuilder.CreateIndex(
                name: "IX_StorePurchase_PurchaseGuid",
                table: "StorePurchase",
                column: "PurchaseGuid");

            migrationBuilder.CreateIndex(
                name: "IX_StorePurchase_StoreGuid",
                table: "StorePurchase",
                column: "StoreGuid");

            migrationBuilder.CreateIndex(
                name: "IX_StorePurchasePurchaseProducts_ProductGuid",
                table: "StorePurchasePurchaseProducts",
                column: "ProductGuid");

            migrationBuilder.CreateIndex(
                name: "IX_StorePurchasePurchaseProducts_StorePurchaseGuid",
                table: "StorePurchasePurchaseProducts",
                column: "StorePurchaseGuid");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_StoreFounderId",
                table: "Stores",
                column: "StoreFounderId");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_StorePolicyGuid",
                table: "Stores",
                column: "StorePolicyGuid");

            migrationBuilder.AddForeignKey(
                name: "FK_StoreManagements_StoreOwnerships_StoreOwnershipGuid",
                table: "StoreManagements",
                column: "StoreOwnershipGuid",
                principalTable: "StoreOwnerships",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoreManagements_Stores_StoreGuid",
                table: "StoreManagements",
                column: "StoreGuid",
                principalTable: "Stores",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StoreOwnerships_Stores_StoreGuid",
                table: "StoreOwnerships",
                column: "StoreGuid",
                principalTable: "Stores",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StorePurchase_Stores_StoreGuid",
                table: "StorePurchase",
                column: "StoreGuid",
                principalTable: "Stores",
                principalColumn: "Guid",
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
                name: "FK_Stores_AspNetUsers_StoreFounderId",
                table: "Stores");

            migrationBuilder.DropForeignKey(
                name: "FK_Policies_Products_ProductGuid",
                table: "Policies");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "PolicyOperators");

            migrationBuilder.DropTable(
                name: "ShoppingBasketPurchaseProducts");

            migrationBuilder.DropTable(
                name: "StoreManagements");

            migrationBuilder.DropTable(
                name: "StorePurchasePurchaseProducts");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "ShoppingBaskets");

            migrationBuilder.DropTable(
                name: "StoreOwnerships");

            migrationBuilder.DropTable(
                name: "StorePurchase");

            migrationBuilder.DropTable(
                name: "ShoppingCarts");

            migrationBuilder.DropTable(
                name: "Purchase");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Stores");

            migrationBuilder.DropTable(
                name: "Policies");
        }
    }
}
