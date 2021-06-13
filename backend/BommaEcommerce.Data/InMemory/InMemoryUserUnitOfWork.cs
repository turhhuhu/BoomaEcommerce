using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Domain.ProductOffer;
using Microsoft.AspNetCore.Identity;

namespace BoomaEcommerce.Data.InMemory
{
    public class InMemoryUserUnitOfWork : IUserUnitOfWork
    {
        public IRepository<ShoppingBasket> ShoppingBasketRepo { get; set; }
        public IRepository<ShoppingCart> ShoppingCartRepo { get; set; }
        public IRepository<ProductOffer> ProductOfferRepo { get; set; }
        public IRepository<PurchaseProduct> PurchaseProductRepo { get; set; }
        public IRepository<User> UserRepository { get; set; }
        public IRepository<Product> ProductRepository { get; set; }

        public InMemoryUserUnitOfWork(
            IRepository<ShoppingBasket> shoppingBasketRepo,
            IRepository<ShoppingCart> shoppingCartRepo,
            IRepository<PurchaseProduct> purchaseProductRepo,
            IRepository<User> userRepository,
            IRepository<ProductOffer> productOfferRepo,
            IRepository<Product> productRepository)
        {
            ShoppingBasketRepo = shoppingBasketRepo;
            ShoppingCartRepo = shoppingCartRepo;
            PurchaseProductRepo = purchaseProductRepo;
            UserRepository = userRepository;
            ProductOfferRepo = productOfferRepo;
            ProductRepository = productRepository;
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
