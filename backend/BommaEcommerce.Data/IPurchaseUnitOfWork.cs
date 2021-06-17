using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Data.EfCore;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Domain.ProductOffer;
using Microsoft.AspNetCore.Identity;

namespace BoomaEcommerce.Data
{
    public interface IPurchaseUnitOfWork
    {
        IRepository<Purchase> PurchaseRepository { get; set; }
        UserManager<User> UserRepository { get; set; }
        IRepository<Product> ProductRepository { get; set; }
        IRepository<ShoppingCart> ShoppingCartRepository { get; set; }
        IRepository<StoreOwnership> StoreOwnershipRepository { get; set; }
        IRepository<ShoppingBasket> ShoppingBasketRepository { get; set; }
        IRepository<Store> StoresRepository { get; set; }
        IRepository<ProductOffer> OffersRepository { get; set; }
        Task SaveAsync();
        void Attach<TEntity>(TEntity entity) where TEntity : class, IBaseEntity;
        Task<ITransactionContext> BeginTransaction();
    }
}
