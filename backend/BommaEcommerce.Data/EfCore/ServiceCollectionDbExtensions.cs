using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Data.EfCore.Repositories;
using BoomaEcommerce.Data.InMemory;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Domain.Discounts;
using BoomaEcommerce.Domain.Policies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using BoomaEcommerce.Domain.ProductOffer;

namespace BoomaEcommerce.Data.EfCore
{
    public static class ServiceCollectionDbExtensions
    {
        public static IServiceCollection AddEfCoreDb(this IServiceCollection services, string connectionString)
        {

            // Ef core
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
            services.AddIdentity<User, IdentityRole<Guid>>(x =>
            {
                x.Password.RequireDigit = false;
                x.Password.RequireLowercase = false;
                x.Password.RequireNonAlphanumeric = false;
                x.Password.RequiredLength = 1;
                x.Password.RequiredUniqueChars = 0;
                x.Password.RequireUppercase = false;
            }).AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddTransient(typeof(IRepository<>), typeof(EfCoreRepository<>));
            services.AddTransient<IRepository<ShoppingCart>, EfCoreRepository<ShoppingCart>>();
            services.AddTransient<IRepository<User>, EfCoreUsersRepository>();
            services.AddTransient<IRepository<StoreOwnership>, EfCoreStoreOwnershipRepository>();
            services.AddTransient<IRepository<Policy>, EfCorePolicyRepository>();
            services.AddTransient<IRepository<Discount>, EfCoreDiscountRepository>();
            services.AddTransient<IRepository<Store>, EfCoreStoreRepository>();
            services.AddTransient<IRepository<RefreshToken>, EfCoreRefreshTokenRepository>();
            services.AddTransient<IRepository<ProductOffer>, EfCoreProductOfferRepository>();

            services.AddTransient<IStoreUnitOfWork, StoreUnitOfWork>();
            services.AddTransient<IPurchaseUnitOfWork, PurchaseUnitOfWork>();
            services.AddTransient<IUserUnitOfWork, UserUnitOfWork>();

            return services;
        }

        public static IServiceCollection AddInMemoryDb(this IServiceCollection services)
        {

            services.AddTransient(_ => new UserManager<User>(
                new InMemoryUserStore(), Options.Create
                    (new IdentityOptions
                    {
                        Stores = new StoreOptions { ProtectPersonalData = false }
                    }),
                new PasswordHasher<User>(),
                new IUserValidator<User>[0],
                new IPasswordValidator<User>[0],
                new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(),
                new DataColumn(),
                new Logger<UserManager<User>>(new LoggerFactory())));

            services.AddTransient(typeof(IRepository<>), typeof(InMemoryRepository<>));
            services.AddTransient<IRepository<StoreOwnership>, InMemoryOwnershipRepository>();
            services.AddTransient<IRepository<StoreManagement>, InMemoryManagementRepository>();
            services.AddTransient<IStoreUnitOfWork, InMemoryStoreUnitOfWork>();
            services.AddTransient<IUserUnitOfWork, InMemoryUserUnitOfWork>();
            services.AddTransient<IPurchaseUnitOfWork, InMemoryPurchaseUnitOfWork>();
            services.AddTransient<IRepository<User>, InMemoryUserRepository>();

            return services;
        }

        public static IServiceCollection AddMixedDb(this IServiceCollection services, string connectionString)
        {

            // In-Memory
            services.AddTransient(typeof(IRepository<>), typeof(InMemoryRepository<>));
            services.AddTransient<IRepository<StoreOwnership>, InMemoryOwnershipRepository>();
            services.AddTransient<IRepository<StoreManagement>, InMemoryManagementRepository>();
            services.AddTransient<IUserUnitOfWork, InMemoryUserUnitOfWork>();
            services.AddTransient<IPurchaseUnitOfWork, InMemoryPurchaseUnitOfWork>();

            // Ef core
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
            services.AddIdentity<User, IdentityRole<Guid>>(x =>
                {
                    x.Password.RequireDigit = false;
                    x.Password.RequireLowercase = false;
                    x.Password.RequireNonAlphanumeric = false;
                    x.Password.RequiredLength = 1;
                    x.Password.RequiredUniqueChars = 0;
                    x.Password.RequireUppercase = false;
                }).AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddTransient<IRepository<Store>, EfCoreRepository<Store>>();
            services.AddTransient<IRepository<Product>, EfCoreRepository<Product>>();
            services.AddTransient<IRepository<Policy>, EfCorePolicyRepository>();
            services.AddTransient<IStoreUnitOfWork, StoreUnitOfWork>();

            services.AddTransient<IRepository<StoreManagement>, EfCoreRepository<StoreManagement>>();
            services.AddTransient<IRepository<StoreOwnership>, EfCoreStoreOwnershipRepository>();
            services.AddTransient<IRepository<ShoppingBasket>, EfCoreRepository<ShoppingBasket>>();
            services.AddTransient<IRepository<User>, EfCoreUsersRepository>();


            services.AddTransient<IRepository<Notification>, EfCoreRepository<Notification>>();
            services.AddTransient<IRepository<Discount>, EfCoreDiscountRepository>();


            //Purchase Unit Of Work 
            services.AddTransient<IRepository<ShoppingCart>, EfCoreRepository<ShoppingCart>>();
            services.AddTransient<IUserUnitOfWork, UserUnitOfWork>();
            services.AddTransient<IPurchaseUnitOfWork, PurchaseUnitOfWork>();
            services.AddTransient<IRepository<Purchase>, EfCoreRepository<Purchase>>();
            
            
            services.AddTransient<IRepository<ProductOffer>, EfCoreRepository<ProductOffer>>();
            services.AddTransient<IRepository<ApproverOwner>, EfCoreRepository<ApproverOwner>>();


            return services;
        }

    }
}
