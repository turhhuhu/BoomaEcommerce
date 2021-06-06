﻿// <auto-generated />
using System;
using BoomaEcommerce.Data.EfCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BoomaEcommerce.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.6")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("BoomaEcommerce.Domain.Notification", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Message")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("WasSeen")
                        .HasColumnType("bit");

                    b.HasKey("Guid");

                    b.HasIndex("UserId");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("BoomaEcommerce.Domain.Policies.Operators.PolicyOperator", b =>
                {
                    b.Property<Guid>("Guid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ErrorPrefix")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Level")
                        .HasColumnType("int");

                    b.Property<string>("PolicyOperatorType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Guid");

                    b.ToTable("PolicyOperators");

                    b.HasDiscriminator<string>("PolicyOperatorType").HasValue("PolicyOperator");
                });

            modelBuilder.Entity("BoomaEcommerce.Domain.Policies.Policy", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("CompositePolicyGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ErrorPrefix")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("FirstPolicyGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Level")
                        .HasColumnType("int");

                    b.Property<string>("PolicyType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("SecondPolicyGuid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Guid");

                    b.HasIndex("CompositePolicyGuid");

                    b.HasIndex("FirstPolicyGuid")
                        .IsUnique()
                        .HasFilter("[FirstPolicyGuid] IS NOT NULL");

                    b.HasIndex("SecondPolicyGuid")
                        .IsUnique()
                        .HasFilter("[SecondPolicyGuid] IS NOT NULL");

                    b.ToTable("Policies");

                    b.HasDiscriminator<string>("PolicyType").HasValue("Policy");
                });

            modelBuilder.Entity("BoomaEcommerce.Domain.Product", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Amount")
                        .HasColumnType("int");

                    b.Property<string>("Category")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<bool>("IsSoftDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasPrecision(10, 5)
                        .HasColumnType("decimal(10,5)");

                    b.Property<decimal>("Rating")
                        .HasPrecision(4, 2)
                        .HasColumnType("decimal(4,2)");

                    b.Property<Guid?>("StoreGuid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Guid");

                    b.HasIndex("StoreGuid");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("BoomaEcommerce.Domain.ShoppingBasket", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ShoppingCartGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("StoreGuid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Guid");

                    b.HasIndex("ShoppingCartGuid");

                    b.HasIndex("StoreGuid");

                    b.ToTable("ShoppingBaskets");
                });

            modelBuilder.Entity("BoomaEcommerce.Domain.ShoppingCart", b =>
                {
                    b.Property<Guid>("Guid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Guid");

                    b.ToTable("ShoppingCarts");
                });

            modelBuilder.Entity("BoomaEcommerce.Domain.Store", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("Rating")
                        .HasColumnType("real");

                    b.Property<Guid?>("StoreFounderId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("StoreName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("StorePolicyGuid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Guid");

                    b.HasIndex("StoreFounderId");

                    b.HasIndex("StorePolicyGuid");

                    b.ToTable("Stores");
                });

            modelBuilder.Entity("BoomaEcommerce.Domain.StoreManagement", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("StoreGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("StoreOwnershipGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Guid");

                    b.HasIndex("StoreGuid");

                    b.HasIndex("StoreOwnershipGuid");

                    b.HasIndex("UserId");

                    b.ToTable("StoreManagements");
                });

            modelBuilder.Entity("BoomaEcommerce.Domain.StoreOwnership", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("StoreGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Guid");

                    b.HasIndex("StoreGuid");

                    b.HasIndex("UserId");

                    b.ToTable("StoreOwnerships");
                });

            modelBuilder.Entity("BoomaEcommerce.Domain.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<Guid>("Guid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole<System.Guid>", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("BoomaEcommerce.Domain.Policies.Operators.AndPolicyOperator", b =>
                {
                    b.HasBaseType("BoomaEcommerce.Domain.Policies.Operators.PolicyOperator");

                    b.HasDiscriminator().HasValue("AndPolicyOperator");
                });

            modelBuilder.Entity("BoomaEcommerce.Domain.Policies.Operators.ConditionPolicyOperator", b =>
                {
                    b.HasBaseType("BoomaEcommerce.Domain.Policies.Operators.PolicyOperator");

                    b.HasDiscriminator().HasValue("ConditionPolicyOperator");
                });

            modelBuilder.Entity("BoomaEcommerce.Domain.Policies.Operators.OrPolicyOperator", b =>
                {
                    b.HasBaseType("BoomaEcommerce.Domain.Policies.Operators.PolicyOperator");

                    b.HasDiscriminator().HasValue("OrPolicyOperator");
                });

            modelBuilder.Entity("BoomaEcommerce.Domain.Policies.Operators.XorPolicyOperator", b =>
                {
                    b.HasBaseType("BoomaEcommerce.Domain.Policies.Operators.PolicyOperator");

                    b.HasDiscriminator().HasValue("XorPolicyOperator");
                });

            modelBuilder.Entity("BoomaEcommerce.Domain.Policies.EmptyPolicy", b =>
                {
                    b.HasBaseType("BoomaEcommerce.Domain.Policies.Policy");

                    b.HasDiscriminator().HasValue("EmptyPolicy");
                });

            modelBuilder.Entity("BoomaEcommerce.Domain.Policies.MultiPolicy", b =>
                {
                    b.HasBaseType("BoomaEcommerce.Domain.Policies.Policy");

                    b.HasDiscriminator().HasValue("MultiPolicy");
                });

            modelBuilder.Entity("BoomaEcommerce.Domain.Policies.PolicyTypes.MaxCategoryAmountPolicy", b =>
                {
                    b.HasBaseType("BoomaEcommerce.Domain.Policies.Policy");

                    b.Property<string>("Category")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("MaxAmount")
                        .HasColumnType("int");

                    b.HasDiscriminator().HasValue("MaxCategoryAmountPolicy");
                });

            modelBuilder.Entity("BoomaEcommerce.Domain.Policies.PolicyTypes.MaxTotalAmountPolicy", b =>
                {
                    b.HasBaseType("BoomaEcommerce.Domain.Policies.Policy");

                    b.Property<int>("MaxAmount")
                        .HasColumnType("int")
                        .HasColumnName("MaxTotalAmountPolicy_MaxAmount");

                    b.HasDiscriminator().HasValue("MaxTotalAmountPolicy");
                });

            modelBuilder.Entity("BoomaEcommerce.Domain.Policies.PolicyTypes.MinCategoryAmountPolicy", b =>
                {
                    b.HasBaseType("BoomaEcommerce.Domain.Policies.Policy");

                    b.Property<string>("Category")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("MinCategoryAmountPolicy_Category");

                    b.Property<int>("MinAmount")
                        .HasColumnType("int");

                    b.HasDiscriminator().HasValue("MinCategoryAmountPolicy");
                });

            modelBuilder.Entity("BoomaEcommerce.Domain.Policies.PolicyTypes.MinTotalAmountPolicy", b =>
                {
                    b.HasBaseType("BoomaEcommerce.Domain.Policies.Policy");

                    b.Property<int>("MinAmount")
                        .HasColumnType("int")
                        .HasColumnName("MinTotalAmountPolicy_MinAmount");

                    b.HasDiscriminator().HasValue("MinTotalAmountPolicy");
                });

            modelBuilder.Entity("BoomaEcommerce.Domain.Policies.ProductPolicy", b =>
                {
                    b.HasBaseType("BoomaEcommerce.Domain.Policies.Policy");

                    b.Property<Guid?>("ProductGuid")
                        .HasColumnType("uniqueidentifier");

                    b.HasIndex("ProductGuid");

                    b.HasDiscriminator().HasValue("ProductPolicy");
                });

            modelBuilder.Entity("BoomaEcommerce.Domain.Policies.BinaryPolicy", b =>
                {
                    b.HasBaseType("BoomaEcommerce.Domain.Policies.MultiPolicy");

                    b.HasDiscriminator().HasValue("BinaryPolicy");
                });

            modelBuilder.Entity("BoomaEcommerce.Domain.Policies.CompositePolicy", b =>
                {
                    b.HasBaseType("BoomaEcommerce.Domain.Policies.MultiPolicy");

                    b.HasDiscriminator().HasValue("CompositePolicy");
                });

            modelBuilder.Entity("BoomaEcommerce.Domain.Policies.PolicyTypes.AgeRestrictionPolicy", b =>
                {
                    b.HasBaseType("BoomaEcommerce.Domain.Policies.ProductPolicy");

                    b.Property<int>("MinAge")
                        .HasColumnType("int");

                    b.HasDiscriminator().HasValue("AgeRestrictionPolicy");
                });

            modelBuilder.Entity("BoomaEcommerce.Domain.Policies.PolicyTypes.MaxProductAmountPolicy", b =>
                {
                    b.HasBaseType("BoomaEcommerce.Domain.Policies.ProductPolicy");

                    b.Property<int>("MaxAmount")
                        .HasColumnType("int")
                        .HasColumnName("MaxProductAmountPolicy_MaxAmount");

                    b.HasDiscriminator().HasValue("MaxProductAmountPolicy");
                });

            modelBuilder.Entity("BoomaEcommerce.Domain.Policies.PolicyTypes.MinProductAmountPolicy", b =>
                {
                    b.HasBaseType("BoomaEcommerce.Domain.Policies.ProductPolicy");

                    b.Property<int>("MinAmount")
                        .HasColumnType("int")
                        .HasColumnName("MinProductAmountPolicy_MinAmount");

                    b.HasDiscriminator().HasValue("MinProductAmountPolicy");
                });

            modelBuilder.Entity("BoomaEcommerce.Domain.Notification", b =>
                {
                    b.HasOne("BoomaEcommerce.Domain.User", null)
                        .WithMany("Notifications")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("BoomaEcommerce.Domain.Policies.Operators.PolicyOperator", b =>
                {
                    b.HasOne("BoomaEcommerce.Domain.Policies.MultiPolicy", null)
                        .WithOne("Operator")
                        .HasForeignKey("BoomaEcommerce.Domain.Policies.Operators.PolicyOperator", "Guid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BoomaEcommerce.Domain.Policies.Policy", b =>
                {
                    b.HasOne("BoomaEcommerce.Domain.Policies.CompositePolicy", null)
                        .WithMany("SubPolicies")
                        .HasForeignKey("CompositePolicyGuid")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("BoomaEcommerce.Domain.Policies.BinaryPolicy", null)
                        .WithOne("FirstPolicy")
                        .HasForeignKey("BoomaEcommerce.Domain.Policies.Policy", "FirstPolicyGuid");

                    b.HasOne("BoomaEcommerce.Domain.Policies.BinaryPolicy", null)
                        .WithOne("SecondPolicy")
                        .HasForeignKey("BoomaEcommerce.Domain.Policies.Policy", "SecondPolicyGuid");
                });

            modelBuilder.Entity("BoomaEcommerce.Domain.Product", b =>
                {
                    b.HasOne("BoomaEcommerce.Domain.Store", "Store")
                        .WithMany()
                        .HasForeignKey("StoreGuid")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Store");
                });

            modelBuilder.Entity("BoomaEcommerce.Domain.ShoppingBasket", b =>
                {
                    b.HasOne("BoomaEcommerce.Domain.ShoppingCart", null)
                        .WithMany("ShoppingBaskets")
                        .HasForeignKey("ShoppingCartGuid")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BoomaEcommerce.Domain.Store", "Store")
                        .WithMany()
                        .HasForeignKey("StoreGuid")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.OwnsMany("BoomaEcommerce.Domain.PurchaseProduct", "PurchaseProducts", b1 =>
                        {
                            b1.Property<Guid>("ShoppingBasketGuid")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int")
                                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                            b1.Property<int>("Amount")
                                .HasColumnType("int");

                            b1.Property<Guid>("Guid")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<decimal>("Price")
                                .HasColumnType("decimal(18,2)");

                            b1.Property<Guid?>("ProductGuid")
                                .HasColumnType("uniqueidentifier");

                            b1.HasKey("ShoppingBasketGuid", "Id");

                            b1.HasIndex("ProductGuid");

                            b1.ToTable("ShoppingBasketPurchaseProducts");

                            b1.HasOne("BoomaEcommerce.Domain.Product", "Product")
                                .WithMany()
                                .HasForeignKey("ProductGuid");

                            b1.WithOwner()
                                .HasForeignKey("ShoppingBasketGuid");

                            b1.Navigation("Product");
                        });

                    b.Navigation("PurchaseProducts");

                    b.Navigation("Store");
                });

            modelBuilder.Entity("BoomaEcommerce.Domain.ShoppingCart", b =>
                {
                    b.HasOne("BoomaEcommerce.Domain.User", "User")
                        .WithOne()
                        .HasForeignKey("BoomaEcommerce.Domain.ShoppingCart", "Guid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("BoomaEcommerce.Domain.Store", b =>
                {
                    b.HasOne("BoomaEcommerce.Domain.User", "StoreFounder")
                        .WithMany()
                        .HasForeignKey("StoreFounderId");

                    b.HasOne("BoomaEcommerce.Domain.Policies.Policy", "StorePolicy")
                        .WithMany()
                        .HasForeignKey("StorePolicyGuid");

                    b.Navigation("StoreFounder");

                    b.Navigation("StorePolicy");
                });

            modelBuilder.Entity("BoomaEcommerce.Domain.StoreManagement", b =>
                {
                    b.HasOne("BoomaEcommerce.Domain.Store", "Store")
                        .WithMany()
                        .HasForeignKey("StoreGuid")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BoomaEcommerce.Domain.StoreOwnership", null)
                        .WithMany("StoreManagements")
                        .HasForeignKey("StoreOwnershipGuid");

                    b.HasOne("BoomaEcommerce.Domain.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Store");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BoomaEcommerce.Domain.StoreOwnership", b =>
                {
                    b.HasOne("BoomaEcommerce.Domain.Store", "Store")
                        .WithMany()
                        .HasForeignKey("StoreGuid");

                    b.HasOne("BoomaEcommerce.Domain.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Store");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<System.Guid>", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.HasOne("BoomaEcommerce.Domain.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.HasOne("BoomaEcommerce.Domain.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<System.Guid>", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BoomaEcommerce.Domain.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.HasOne("BoomaEcommerce.Domain.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BoomaEcommerce.Domain.Policies.ProductPolicy", b =>
                {
                    b.HasOne("BoomaEcommerce.Domain.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductGuid")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("Product");
                });

            modelBuilder.Entity("BoomaEcommerce.Domain.ShoppingCart", b =>
                {
                    b.Navigation("ShoppingBaskets");
                });

            modelBuilder.Entity("BoomaEcommerce.Domain.StoreOwnership", b =>
                {
                    b.Navigation("StoreManagements");
                });

            modelBuilder.Entity("BoomaEcommerce.Domain.User", b =>
                {
                    b.Navigation("Notifications");
                });

            modelBuilder.Entity("BoomaEcommerce.Domain.Policies.MultiPolicy", b =>
                {
                    b.Navigation("Operator");
                });

            modelBuilder.Entity("BoomaEcommerce.Domain.Policies.BinaryPolicy", b =>
                {
                    b.Navigation("FirstPolicy");

                    b.Navigation("SecondPolicy");
                });

            modelBuilder.Entity("BoomaEcommerce.Domain.Policies.CompositePolicy", b =>
                {
                    b.Navigation("SubPolicies");
                });
#pragma warning restore 612, 618
        }
    }
}
