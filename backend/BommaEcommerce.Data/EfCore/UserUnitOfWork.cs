using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Domain;
using Microsoft.AspNetCore.Identity;

namespace BoomaEcommerce.Data.EfCore
{
    public class UserUnitOfWork : IUserUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        public IRepository<ShoppingBasket> ShoppingBasketRepo { get; set; }
        public IRepository<ShoppingCart> ShoppingCartRepo { get; set; }
        public UserManager<User> UserManager { get; set; }

        public UserUnitOfWork(
            ApplicationDbContext dbContext,
            IRepository<ShoppingBasket> shoppingBasketRepo,
            IRepository<ShoppingCart> shoppingCartRepo,
            IRepository<PurchaseProduct> purchaseProductRepo,
            UserManager<User> userManager 
            )
        {
            _dbContext = dbContext;
            ShoppingBasketRepo = shoppingBasketRepo;
            ShoppingCartRepo = shoppingCartRepo;
            UserManager = userManager;

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
        public void AttachNoChange<TEntity>(TEntity entity)
            where TEntity : class, IBaseEntity
        {
            _dbContext.Set<TEntity>().Attach(entity);
        }

    }
}
