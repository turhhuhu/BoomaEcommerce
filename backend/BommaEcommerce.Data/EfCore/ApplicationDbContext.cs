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

            //modelBuilder.Entity<ShoppingCart>(sc =>
            //{
            //    sc.HasKey(s => s.Guid);
            //    sc.HasOne(s => s.User).WithOne().HasForeignKey<User>(x => x.Guid);
            //    // sc.Ignore(s => s.User);
            //    sc.Ignore(s => s.StoreGuidToBaskets);

            //});


            modelBuilder.Entity<User>()
                .HasMany(u => u.Notifications)
                .WithOne();

            modelBuilder.Entity<Notification>().HasKey(n => n.Guid);

            //modelBuilder.Ignore<EmptyPolicy>();
            modelBuilder.Entity<Policy>(p =>
            {
                p.HasKey(pp => pp.Guid);
                p.HasDiscriminator<string>("PolicyType")
                    .HasValue<AgeRestrictionPolicy>(nameof(AgeRestrictionPolicy))
                    .HasValue<MaxCategoryAmountPolicy>(nameof(MaxCategoryAmountPolicy))
                    .HasValue<MinCategoryAmountPolicy>(nameof(MinCategoryAmountPolicy))
                    .HasValue<MaxTotalAmountPolicy>(nameof(MaxTotalAmountPolicy))
                    .HasValue<MinTotalAmountPolicy>(nameof(MinTotalAmountPolicy))
                    .HasValue<MaxProductAmountPolicy>(nameof(MaxProductAmountPolicy))
                    .HasValue<MinProductAmountPolicy>(nameof(MinProductAmountPolicy))
                    .HasValue<MultiPolicy>(nameof(MultiPolicy));
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


            modelBuilder.Entity<AgeRestrictionPolicy>()
                .HasOne(p => p.Product)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<MaxCategoryAmountPolicy>();
            modelBuilder.Entity<MinCategoryAmountPolicy>();
            modelBuilder.Entity<MaxTotalAmountPolicy>();
            modelBuilder.Entity<MinTotalAmountPolicy>();

            modelBuilder.Entity<MaxProductAmountPolicy>()
                .HasOne(p => p.Product)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<MinProductAmountPolicy>()
                .HasOne(p => p.Product)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<CompositePolicy>(p =>
            {
                p.HasMany(pp => pp.SubPolicies)
                    .WithOne()
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<BinaryPolicy>(p =>
            {
                p.HasOne(pp => pp.FirstPolicy);

                p.HasOne(pp => pp.SecondPolicy);

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


            base.OnModelCreating(modelBuilder);
        }
    }
}
