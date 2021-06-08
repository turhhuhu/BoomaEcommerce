using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Domain;
using Microsoft.AspNetCore.Identity;

namespace BoomaEcommerce.Data.InMemory
{
    public class InMemoryUserUnitOfWork : IUserUnitOfWork
    {
        public IRepository<ShoppingBasket> ShoppingBasketRepo { get; set; }
        public IRepository<ShoppingCart> ShoppingCartRepo { get; set; }
        public IRepository<PurchaseProduct> PurchaseProductRepo { get; set; }
        public UserManager<User> UserManager { get; set; }

        public InMemoryUserUnitOfWork(
            IRepository<ShoppingBasket> shoppingBasketRepo,
            IRepository<ShoppingCart> shoppingCartRepo,
            IRepository<PurchaseProduct> purchaseProductRepo,
            UserManager<User> userManager)
        {
            ShoppingBasketRepo = shoppingBasketRepo;
            ShoppingCartRepo = shoppingCartRepo;
            PurchaseProductRepo = purchaseProductRepo;
            UserManager = userManager;
        }

        public Task SaveAsync()
        {
            return Task.CompletedTask;
        }

        public void AttachNoChange<TEntity>(TEntity entity) where TEntity : class, IBaseEntity
        { 
        }
    }
}
