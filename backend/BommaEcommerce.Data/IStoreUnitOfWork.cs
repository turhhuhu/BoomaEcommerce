using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Domain;

namespace BoomaEcommerce.Data
{
    public interface IStoreUnitOfWork
    {
        IRepository<Store> StoreRepo { get; set; }
        IRepository<StoreOwnership> StoreOwnershipRepo { get; set; }
        IRepository<StorePurchase> StorePurchaseRepo { get; set; }
        IRepository<StoreManagement> StoreManagementRepo { get; set; }
        IRepository<StoreManagementPermission> StoreManagementPermissionsRepo { get; set; }
        IRepository<Product> ProductRepo { get; set; }
        Task SaveAsync();
    }
}
