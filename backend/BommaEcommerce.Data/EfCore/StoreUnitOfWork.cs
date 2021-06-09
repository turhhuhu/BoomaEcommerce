using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Domain.Discounts;
using BoomaEcommerce.Domain.Policies;
using Microsoft.EntityFrameworkCore;

namespace BoomaEcommerce.Data.EfCore
{
    public class StoreUnitOfWork : IStoreUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        public IRepository<Store> StoreRepo { get; set; }
        public IRepository<StoreOwnership> StoreOwnershipRepo { get; set; }
        public IRepository<StorePurchase> StorePurchaseRepo { get; set; }
        public IRepository<StoreManagement> StoreManagementRepo { get; set; }
        public IRepository<StoreManagementPermissions> StoreManagementPermissionsRepo { get; set; }
        public IRepository<Product> ProductRepo { get; set; }
        public IRepository<Policy> PolicyRepo { get; set; }
        public IRepository<Discount> DiscountRepo { get; set; }
        public IRepository<User> UserRepo { get; set; }


        public StoreUnitOfWork(
            ApplicationDbContext dbContext,
            IRepository<Store> storeRepo,
            IRepository<StoreOwnership> ownershipRepo,
            IRepository<StorePurchase> purchaseRepo,
            IRepository<StoreManagement> storeManagementRepo,
            IRepository<StoreManagementPermissions> storeManagementPermissionsRepo,
            IRepository<Product> productRepo,
            IRepository<Policy> policyRepo,
            IRepository<User> userRepo)
        {
            _dbContext = dbContext;
            StoreRepo = storeRepo;
            StoreOwnershipRepo = ownershipRepo;
            StorePurchaseRepo = purchaseRepo;
            StoreManagementRepo = storeManagementRepo;
            StoreManagementPermissionsRepo = storeManagementPermissionsRepo;
            ProductRepo = productRepo;
            PolicyRepo = policyRepo;
            UserRepo = userRepo;
        }


        public Task SaveAsync()
        {
            return _dbContext.SaveChangesAsync();
        }

        public void AttachNoChange<TEntity>(TEntity entity)
            where TEntity : class, IBaseEntity
        {
            _dbContext.Set<TEntity>().Attach(entity);
        }

        public void Attach<TEntity>(TEntity entity) where TEntity : class, IBaseEntity
        {
            entity.Guid = default;
            _dbContext.Attach(entity);
        }

        public async Task AttachUser(User user)
        {
            if (!string.IsNullOrEmpty(user.UserName))
            {
                await _dbContext.Set<User>().FirstOrDefaultAsync(u => u.UserName == user.UserName);
                return;
            }

            _dbContext.Set<User>().Attach(user);
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
    }

}
