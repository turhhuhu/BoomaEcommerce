using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Domain.Discounts;
using BoomaEcommerce.Domain.Policies;
using BoomaEcommerce.Domain.ProductOffer;

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
        public IRepository<User> UserRepo { get; set; }
        public IRepository<ProductOffer> OffersRepo { get; set; }
        public IRepository<Discount> DiscountRepo { get; set; }
        public IRepository<ApproverOwner> ApproversRepo { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public InMemoryStoreUnitOfWork(
            IRepository<Store> storeRepo,
            IRepository<StoreOwnership> ownershipRepo,
            IRepository<StorePurchase> purchaseRepo,
            IRepository<StoreManagement> storeManagementRepo,
            IRepository<StoreManagementPermissions> storeManagementPermissionsRepo,
            IRepository<Product> productRepo,
            IRepository<Policy> policyRepo,
            IRepository<Discount> discountRepo,
            IRepository<User> userRepo,
            IRepository<ProductOffer> offersRepo,
            IRepository<ApproverOwner> approversRepo)
        {
            StoreRepo = storeRepo;
            StoreOwnershipRepo = ownershipRepo;
            StorePurchaseRepo = purchaseRepo;
            StoreManagementRepo = storeManagementRepo;
            StoreManagementPermissionsRepo = storeManagementPermissionsRepo;
            ProductRepo = productRepo;
            PolicyRepo = policyRepo;
            DiscountRepo = discountRepo;
            UserRepo = userRepo;
            OffersRepo = offersRepo;
            ApproversRepo = approversRepo;
        }

        public Task SaveAsync()
        {
            return Task.CompletedTask;
        }

        public void AttachNoChange<TEntity>(TEntity entity)
            where TEntity : class, IBaseEntity
        {
            
        }

        public void Attach<TEntity>(TEntity entity) where TEntity : class, IBaseEntity
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
