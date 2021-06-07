using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Domain.Policies;
using BoomaEcommerce.Domain.Policies.Operators;
using BoomaEcommerce.Domain.Policies.PolicyTypes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace BoomaEcommerce.Data.EfCore
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Policy> Policies { get; set; }
        public DbSet<ShoppingBasket> ShoppingBaskets { get; set; }
        public DbSet<StoreManagement> StoreManagements {get; set;}
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<StoreOwnership> StoreOwnerships { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(p =>
            {
                p.Property(pp => pp.Price).HasPrecision(10, 5);
                p.Property(pp => pp.Rating).HasPrecision(4, 2);
                p.HasKey(pp => pp.Guid);
                p.Ignore(pp => pp.ProductLock)
                    .HasOne(pp => pp.Store)
                    .WithMany()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Store>(s =>
            {
                s.HasOne(ss => ss.StoreFounder)
                    .WithMany();

                s.HasOne(ss => ss.StorePolicy);

                s.HasKey(ss => ss.Guid);
            });


            modelBuilder.Entity<User>()
                .HasMany(u => u.Notifications)
                .WithOne();

            modelBuilder.Entity<StoreManagement>(sm =>
            {
                sm.HasOne(s => s.User).WithMany().OnDelete(DeleteBehavior.Cascade); 
                sm.HasOne(s => s.Store).WithMany().OnDelete(DeleteBehavior.Cascade);
                sm.Ignore(s => s.Permissions);
                sm.HasKey(s => s.Guid);
            });

            
            modelBuilder.Entity<StoreOwnership>(so =>
            {
                so.HasMany(s => s.StoreManagements).WithOne();
                //so.Ignore(s => s.StoreManagements);
                //so.HasMany(s => s.StoreOwnerships).WithOne(); 
                so.Ignore(s => s.StoreOwnerships);
                so.HasOne(s => s.Store).WithMany();
                //so.Ignore(s => s.Store);
                so.HasOne(s => s.User).WithMany().OnDelete(DeleteBehavior.Cascade);
                //so.Ignore(s => s.User);
                so.HasKey(s=>s.Guid);
            }); 

       

            modelBuilder.Entity<Notification>().HasKey(n => n.Guid);

            AddPolicyModels(modelBuilder);
            AddCartModels(modelBuilder);

            modelBuilder.Entity<Purchase>(p =>
            {
                p.HasKey(pp => pp.Guid);
                p.HasMany(pp => pp.StorePurchases)
                    .WithOne();

                // TODO: Change to be a new object which is information of user\guest
                p.HasOne(pp => pp.Buyer)
                    .WithMany();
            });

            modelBuilder.Entity<StorePurchase>(sp =>
            {
                sp.HasKey(s => s.Guid);
                sp.OwnsMany(s => s.PurchaseProducts, p =>
                {
                    p.HasKey(pp => pp.Guid);
                    p.HasOne(x => x.Product)
                        .WithMany();
                    p.WithOwner();
                    p.ToTable("StorePurchasePurchaseProducts");

                    p.Property(pp => pp.Price).HasPrecision(10, 5);
                });
                sp.HasOne(s => s.Store)
                    .WithMany();

                // TODO: Change to be a new object which is information of user\guest
                sp.HasOne(s => s.Buyer)
                    .WithMany();

                sp.Property(s => s.TotalPrice).HasPrecision(10, 5);
            });


            base.OnModelCreating(modelBuilder);
        }

        private void AddCartModels(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShoppingCart>(sc =>
            {
                sc.HasKey(s => s.Guid);
                sc.HasOne(s => s.User)
                    .WithOne()
                    .HasForeignKey<ShoppingCart>(x => x.Guid);

                sc.HasMany(s => s.ShoppingBaskets)
                    .WithOne()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ShoppingBasket>(sb =>
            {
                sb.HasKey(b => b.Guid);
                sb.OwnsMany(b => b.PurchaseProducts)
                    .ToTable("ShoppingBasketPurchaseProducts")
                    .HasOne(x => x.Product)
                    .WithMany();

                sb.OwnsMany(b => b.PurchaseProducts, p =>
                {
                    p.HasKey(pp => pp.Guid);
                    p.HasOne(x => x.Product)
                        .WithMany();
                    p.WithOwner();
                    p.Property(pp => pp.Price).HasPrecision(10, 5);
                    p.ToTable("ShoppingBasketPurchaseProducts");
                });
            });
        }
        private void AddPolicyModels(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<EmptyPolicy>();
            modelBuilder.Entity<Policy>(p =>
            {
                p.HasKey(pp => pp.Guid);
                p.HasDiscriminator<string>("PolicyType")
                    .HasValue<MaxCategoryAmountPolicy>(nameof(MaxCategoryAmountPolicy))
                    .HasValue<MinCategoryAmountPolicy>(nameof(MinCategoryAmountPolicy))
                    .HasValue<MaxTotalAmountPolicy>(nameof(MaxTotalAmountPolicy))
                    .HasValue<MinTotalAmountPolicy>(nameof(MinTotalAmountPolicy))
                    .HasValue<ProductPolicy>(nameof(ProductPolicy))
                    .HasValue<MultiPolicy>(nameof(MultiPolicy))
                    .HasValue<EmptyPolicy>(nameof(EmptyPolicy));
            });

            modelBuilder.Entity<ProductPolicy>(p =>
            {
                p.HasDiscriminator<string>("PolicyType")
                    .HasValue<AgeRestrictionPolicy>(nameof(AgeRestrictionPolicy))
                    .HasValue<MaxProductAmountPolicy>(nameof(MaxProductAmountPolicy))
                    .HasValue<MinProductAmountPolicy>(nameof(MinProductAmountPolicy));

                p.HasOne(pp => pp.Product)
                    .WithMany()
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<MultiPolicy>(p =>
            {
                p.HasDiscriminator<string>("PolicyType")
                    .HasValue<CompositePolicy>(nameof(CompositePolicy))
                    .HasValue<BinaryPolicy>(nameof(BinaryPolicy));

                p.HasOne(pp => pp.Operator)
                    .WithOne()
                    .HasForeignKey<PolicyOperator>(op => op.Guid)
                    .OnDelete(DeleteBehavior.Cascade);
            });


            modelBuilder.Entity<AgeRestrictionPolicy>();
            modelBuilder.Entity<MaxCategoryAmountPolicy>();
            modelBuilder.Entity<MinCategoryAmountPolicy>();
            modelBuilder.Entity<MaxTotalAmountPolicy>();
            modelBuilder.Entity<MinTotalAmountPolicy>();

            modelBuilder.Entity<MaxProductAmountPolicy>();

            modelBuilder.Entity<MinProductAmountPolicy>();

            modelBuilder.Entity<CompositePolicy>(p =>
            {
                p.HasMany(pp => pp.SubPolicies)
                    .WithOne()
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<BinaryPolicy>(p =>
            {
                p.HasOne(pp => pp.FirstPolicy)
                    .WithOne()
                    .HasForeignKey<Policy>("FirstPolicyGuid");

                p.HasOne(pp => pp.SecondPolicy)
                    .WithOne()
                    .HasForeignKey<Policy>("SecondPolicyGuid");

                var firstPolicyMetaData = p.Metadata.FindNavigation(nameof(BinaryPolicy.FirstPolicy));
                firstPolicyMetaData.SetField("_firstPolicy");
                firstPolicyMetaData.SetPropertyAccessMode(PropertyAccessMode.Field);

                var secondPolicyMetaData = p.Metadata.FindNavigation(nameof(BinaryPolicy.SecondPolicy));
                secondPolicyMetaData.SetField("_secondPolicy");
                secondPolicyMetaData.SetPropertyAccessMode(PropertyAccessMode.Field);

                p.HasOne(pp => pp.Operator);
            });

            modelBuilder.Entity<PolicyOperator>(op =>
            {
                op.HasKey(opp => opp.Guid);
                op.HasDiscriminator<string>("PolicyOperatorType")
                    .HasValue<OrPolicyOperator>(nameof(OrPolicyOperator))
                    .HasValue<AndPolicyOperator>(nameof(AndPolicyOperator))
                    .HasValue<XorPolicyOperator>(nameof(XorPolicyOperator))
                    .HasValue<ConditionPolicyOperator>(nameof(ConditionPolicyOperator));
                op.ToTable("PolicyOperators");
            });

            modelBuilder.Entity<OrPolicyOperator>();
            modelBuilder.Entity<AndPolicyOperator>();
            modelBuilder.Entity<XorPolicyOperator>();
            modelBuilder.Entity<ConditionPolicyOperator>();

        }
    }
}
