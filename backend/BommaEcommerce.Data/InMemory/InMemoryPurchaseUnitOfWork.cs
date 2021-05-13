using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Domain;
using Microsoft.AspNetCore.Identity;

namespace BoomaEcommerce.Data.InMemory
{
    public class InMemoryPurchaseUnitOfWork : IPurchaseUnitOfWork
    {
        public InMemoryPurchaseUnitOfWork(
            IRepository<Purchase> purchaseRepository,
            UserManager<User> userRepository,
            IRepository<Product> productRepository,
            IRepository<ShoppingCart> shoppingCartRepository,
            IRepository<StoreOwnership> storeOwnershipRepository)
        {
            PurchaseRepository = purchaseRepository;
            UserRepository = userRepository;
            ProductRepository = productRepository;
            ShoppingCartRepository = shoppingCartRepository;
            StoreOwnershipRepository = storeOwnershipRepository;
        }
        public IRepository<Purchase> PurchaseRepository { get; set; }
        public UserManager<User> UserRepository { get; set; }
        public IRepository<Product> ProductRepository { get; set; }
        public IRepository<ShoppingCart> ShoppingCartRepository { get; set; }
        public IRepository<StoreOwnership> StoreOwnershipRepository { get; set; }
        public Task SaveAsync()
        {
            return Task.CompletedTask;
        }
    }
}
