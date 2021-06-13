﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Domain;
using Microsoft.AspNetCore.Identity;

namespace BoomaEcommerce.Data.EfCore
{
    public class PurchaseUnitOfWork : IPurchaseUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        public IRepository<Purchase> PurchaseRepository { get; set; }
        public UserManager<User> UserRepository { get; set; }
        public IRepository<Product> ProductRepository { get; set; }
        public IRepository<ShoppingCart> ShoppingCartRepository { get; set; }
        public IRepository<StoreOwnership> StoreOwnershipRepository { get; set; }
        public IRepository<Store> StoresRepository { get; set; }

        public PurchaseUnitOfWork(ApplicationDbContext dbContext,
                                  IRepository<Purchase> purchaseRepo,
                                  UserManager<User> userRepo,
                                  IRepository<Product> productRepo,
                                  IRepository<ShoppingCart> shoppingCartRepo,
                                  IRepository<StoreOwnership> storeOwnerShipRepo,
                                  IRepository<Store> storeRepo)
        {
            _dbContext = dbContext;
            PurchaseRepository = purchaseRepo;
            UserRepository = userRepo;
            ProductRepository = productRepo;
            ShoppingCartRepository = shoppingCartRepo;
            StoreOwnershipRepository = storeOwnerShipRepo;
            StoresRepository = storeRepo;
        }

        public Task SaveAsync()
        {
            return _dbContext.SaveChangesAsync();
        }

        public void Attach<TEntity>(TEntity entity) where TEntity : class, IBaseEntity
        {
            entity.Guid = default;
            _dbContext.Attach(entity);
        }

        public async Task<ITransactionContext> BeginTransaction()
        {
            var transaction = await _dbContext.Database.BeginTransactionAsync();
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
                _dbContext.Dispose();
            }
        }
        public void AttachNoChange<TEntity>(TEntity entity)
            where TEntity : class, IBaseEntity
        {
            _dbContext.Set<TEntity>().Attach(entity);
        }
    }
}
