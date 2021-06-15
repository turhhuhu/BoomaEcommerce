using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Domain.ProductOffer;
using Microsoft.AspNetCore.Identity;

namespace BoomaEcommerce.Data.EfCore
{
    public class PurchaseUnitOfWork : IPurchaseUnitOfWork
    {
        protected readonly ApplicationDbContext DbContext;
        public IRepository<Purchase> PurchaseRepository { get; set; }
        public UserManager<User> UserRepository { get; set; }
        public IRepository<Product> ProductRepository { get; set; }
        public IRepository<ShoppingCart> ShoppingCartRepository { get; set; }
        public IRepository<StoreOwnership> StoreOwnershipRepository { get; set; }
        public IRepository<ShoppingBasket> ShoppingBasketRepository { get; set; }
        public IRepository<Store> StoresRepository { get; set; }
        public IRepository<ProductOffer> OffersRepository { get; set; }

        public PurchaseUnitOfWork(ApplicationDbContext dbContext,
                                  IRepository<Purchase> purchaseRepo,
                                  UserManager<User> userRepo,
                                  IRepository<Product> productRepo,
                                  IRepository<ShoppingCart> shoppingCartRepo,
                                  IRepository<StoreOwnership> storeOwnerShipRepo,
                                  IRepository<Store> storeRepo, 
                                  IRepository<ProductOffer> offersRepo,
                                  IRepository<ShoppingBasket> shoppingBasketRepo)
        {
            DbContext = dbContext;
            PurchaseRepository = purchaseRepo;
            UserRepository = userRepo;
            ProductRepository = productRepo;
            ShoppingCartRepository = shoppingCartRepo;
            StoreOwnershipRepository = storeOwnerShipRepo;
            StoresRepository = storeRepo;
            OffersRepository = offersRepo;
            ShoppingBasketRepository = shoppingBasketRepo;
        }

        public Task SaveAsync()
        {
            return DbContext.SaveChangesAsync();
        }

        public virtual void Attach<TEntity>(TEntity entity) where TEntity : class, IBaseEntity
        {
            entity.Guid = default;
            DbContext.Attach(entity);
        }

        public virtual async Task<ITransactionContext> BeginTransaction()
        {
            var transaction = await DbContext.Database.BeginTransactionAsync();
            return new TransactionContext(transaction);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                DbContext.Dispose();
            }
        }
        public void AttachNoChange<TEntity>(TEntity entity)
            where TEntity : class, IBaseEntity
        {
            DbContext.Set<TEntity>().Attach(entity);
        }
    }
}
