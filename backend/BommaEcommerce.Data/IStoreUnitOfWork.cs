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

namespace BoomaEcommerce.Data
{
    public interface IStoreUnitOfWork : IDisposable
    {
        IRepository<Store> StoreRepo { get; set; }
        IRepository<StoreOwnership> StoreOwnershipRepo { get; set; }
        IRepository<StorePurchase> StorePurchaseRepo { get; set; }
        IRepository<StoreManagement> StoreManagementRepo { get; set; }
        IRepository<Product> ProductRepo { get; set; }
        IRepository<Policy> PolicyRepo { get; set; }
        IRepository<Discount> DiscountRepo { get; set; }
        IRepository<User> UserRepo { get; set; }
        IRepository<ProductOffer> OffersRepo { get; set; }
        IRepository<ApproverOwner> ApproversRepo { get; set; }
        Task SaveAsync();
        void AttachNoChange<TEntity>(TEntity entity) where TEntity : class, IBaseEntity;

        void Attach<TEntity>(TEntity entity) where TEntity : class, IBaseEntity;
    }
}
