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
    public class UserUnitOfWork : IUserUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        public IRepository<ShoppingBasket> ShoppingBasketRepo { get; set; }
        public IRepository<ShoppingCart> ShoppingCartRepo { get; set; }
        public IRepository<ProductOffer> ProductOfferRepo { get; set; }
        public IRepository<User> UserRepository { get; set; }
        public IRepository<Product> ProductRepository { get; set; }
        public IRepository<ApproverOwner> ApproversRepo { get; set ; }
        public IRepository<StoreOwnership> StoreOwnershipRepo { get; set; }


        public UserUnitOfWork(
            ApplicationDbContext dbContext,
            IRepository<ShoppingBasket> shoppingBasketRepo,
            IRepository<ShoppingCart> shoppingCartRepo,
            IRepository<PurchaseProduct> purchaseProductRepo,
            IRepository<User> userRepo,
            IRepository<ProductOffer> productOfferRepo,
            IRepository<Product> productRepository,
            IRepository<ApproverOwner> approversRepo,
            IRepository<StoreOwnership> storeOwnershipRepo
            )
        {
            _dbContext = dbContext;
            ShoppingBasketRepo = shoppingBasketRepo;
            ShoppingCartRepo = shoppingCartRepo;
            UserRepository = userRepo;
            ProductOfferRepo = productOfferRepo;
            ProductRepository = productRepository;
            ApproversRepo = approversRepo;
            StoreOwnershipRepo = storeOwnershipRepo;
        }


        public Task SaveAsync()
        {
            return _dbContext.SaveChangesAsync();
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
                _dbContext.Dispose();
            }
        }
        public void Attach<TEntity>(TEntity entity) where TEntity : class, IBaseEntity
        {
            _dbContext.Add(entity);
        }
        public void AttachNoChange<TEntity>(TEntity entity)
            where TEntity : class, IBaseEntity
        {
            _dbContext.Set<TEntity>().Attach(entity);
        }

    }
}
