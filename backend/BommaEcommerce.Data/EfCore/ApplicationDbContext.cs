using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BoomaEcommerce.Data.EfCore
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Store> Stores { get; set; }

        public DbSet<StoreManagement> StoreManagements {get; set;}

        public DbSet<ShoppingCart> ShoppingCarts { get; set; }

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
                s.Ignore(ss => ss.StorePolicy)
                    .HasOne(ss => ss.StoreFounder)
                    .WithMany();

                s.HasKey(ss => ss.Guid);
            });

            modelBuilder.Entity<ShoppingCart>(sc =>
            {
                sc.HasKey(s => s.Guid);
                sc.HasOne(s => s.User).WithOne().HasForeignKey<User>(x => x.Guid);
                // sc.Ignore(s => s.User);
                sc.Ignore(s => s.StoreGuidToBaskets);

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

            /*
            modelBuilder.Entity<StoreOwnership>(so =>
            {
                so.HasMany(s => s.StoreManagements.Values).WithOne();
                so.HasMany(s => s.StoreOwnerships.Values).WithOne(); 
                so.HasOne(s => s.Store).WithMany(); 
                so.HasOne(s => s.User).WithMany();
                so.HasKey(s=>s.Guid);
            });*/

            modelBuilder.Entity<ShoppingCart>(sc =>
            {
                sc.HasKey(s => s.Guid);
                sc.HasOne(s => s.User).WithOne().HasForeignKey<ShoppingCart>(x => x.Guid);
                // sc.Ignore(s => s.User);
                sc.Ignore(s => s.StoreGuidToBaskets);

            });

            modelBuilder.Entity<Notification>().HasKey(n => n.Guid);
            base.OnModelCreating(modelBuilder);
        }

    }
}
