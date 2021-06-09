using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Domain.Discounts;
using BoomaEcommerce.Domain.Policies;

namespace BoomaEcommerce.Data.InMemory
{
    public class InMemoryStoreUnitOfWork : IStoreUnitOfWork
    {
        public IRepository<Store> StoreRepo { get; set; }
        public IRepository<StoreOwnership> StoreOwnershipRepo { get; set; }
        public IRepository<StorePurchase> StorePurchaseRepo { get; set; }
        public IRepository<StoreManagement> StoreManagementRepo { get; set; }
        public IRepository<StoreManagementPermissions> StoreManagementPermissionsRepo { get; set; }
        public IRepository<Product> ProductRepo { get; set; }
        public IRepository<Policy> PolicyRepo { get; set; }
        public IRepository<Discount> DiscountRepo { get; set; }

        public InMemoryStoreUnitOfWork(
            IRepository<Store> storeRepo,
            IRepository<StoreOwnership> ownershipRepo,
            IRepository<StorePurchase> purchaseRepo,
            IRepository<StoreManagement> storeManagementRepo,
            IRepository<StoreManagementPermissions> storeManagementPermissionsRepo,
            IRepository<Product> productRepo,
            IRepository<Policy> policyRepo,
            IRepository<Discount> discountRepo)
        {
            StoreRepo = storeRepo;
            StoreOwnershipRepo = ownershipRepo;
            StorePurchaseRepo = purchaseRepo;
            StoreManagementRepo = storeManagementRepo;
            StoreManagementPermissionsRepo = storeManagementPermissionsRepo;
            ProductRepo = productRepo;
            PolicyRepo = policyRepo;
            DiscountRepo = discountRepo;
        }

        public Task SaveAsync()
        {
            return Task.CompletedTask;
        }

        public void AttachNoChange<TEntity>(TEntity entity)
            where TEntity : class
        {
            
        }

        public Task AttachUser(User user)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }
    }
}
