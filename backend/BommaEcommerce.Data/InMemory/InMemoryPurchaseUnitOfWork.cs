using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Data.EfCore;
using BoomaEcommerce.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;

namespace BoomaEcommerce.Data.InMemory
{
    public class InMemoryPurchaseUnitOfWork : IPurchaseUnitOfWork
    {
        public InMemoryPurchaseUnitOfWork(
            IRepository<Purchase> purchaseRepository,
            IRepository<User> userRepository,
            IRepository<Product> productRepository,
            IRepository<ShoppingCart> shoppingCartRepository,
            IRepository<StoreOwnership> storeOwnershipRepository,
            IRepository<Store> storesRepository)
        {
            PurchaseRepository = purchaseRepository;
            UserRepository = userRepository;
            ProductRepository = productRepository;
            ShoppingCartRepository = shoppingCartRepository;
            StoreOwnershipRepository = storeOwnershipRepository;
            StoresRepository = storesRepository;

        }
        public IRepository<Purchase> PurchaseRepository { get; set; }
        public IRepository<User> UserRepository { get; set; }
        public IRepository<Product> ProductRepository { get; set; }
        public IRepository<ShoppingCart> ShoppingCartRepository { get; set; }
        public IRepository<StoreOwnership> StoreOwnershipRepository { get; set; }
        public IRepository<Store> StoresRepository { get; set; }

        public Task SaveAsync()
        {
            return Task.CompletedTask;
        }

        public void Attach<TEntity>(TEntity entity) where TEntity : class, IBaseEntity
        {
            
        }

        public Task<ITransactionContext> BeginTransaction()
        {
            return Task.FromResult<ITransactionContext>(new StubTransactionContext());
        }

        public class StubTransactionContext : ITransactionContext
        {
            public void Dispose()
            {
            }

            public Task CommitAsync()
            {
                return Task.CompletedTask;
            }
        }
    }
}
