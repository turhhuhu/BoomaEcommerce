using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Domain.Policies;

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


        public StoreUnitOfWork(
            ApplicationDbContext dbContext,
            IRepository<Store> storeRepo,
            IRepository<StoreOwnership> ownershipRepo,
            IRepository<StorePurchase> purchaseRepo,
            IRepository<StoreManagement> storeManagementRepo,
            IRepository<StoreManagementPermissions> storeManagementPermissionsRepo,
            IRepository<Product> productRepo,
            IRepository<Policy> policyRepo)
        {
            _dbContext = dbContext;
            StoreRepo = storeRepo;
            StoreOwnershipRepo = ownershipRepo;
            StorePurchaseRepo = purchaseRepo;
            StoreManagementRepo = storeManagementRepo;
            StoreManagementPermissionsRepo = storeManagementPermissionsRepo;
            ProductRepo = productRepo;
            PolicyRepo = policyRepo;
        }


        public Task SaveAsync()
        {
            var emptyPolicyStores = _dbContext.Set<Store>().Local.Where(s => s.StorePolicy is EmptyPolicy).ToList();
            emptyPolicyStores.ForEach(s => s.StorePolicy = null);
            return _dbContext.SaveChangesAsync();
        }

        public void AttachNoChange<TEntity>(TEntity entity)
            where TEntity : class
        {
            _dbContext.Set<TEntity>().Attach(entity);
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
