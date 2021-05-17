using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Domain;

namespace BoomaEcommerce.Data.InMemory
{
    public class InMemorySellerRepository : InMemoryRepository<StoreOwnership>
    {
        public override Task InsertOneAsync(StoreOwnership entity)
        {
            var stores = RepoContainer.AllEntities[typeof(Store)];
            var users = InMemoryUserStore.Users;
            entity.Store = (Store)stores[entity.Store.Guid];
            entity.User = users[entity.User.Guid.ToString()];
            return base.InsertOneAsync(entity);
        }
    }
    public class InMemoryManagementRepository : InMemoryRepository<StoreManagement>
    {

        public override Task InsertOneAsync(StoreManagement entity)
        {
            var stores = RepoContainer.AllEntities[typeof(Store)];
            var users = InMemoryUserStore.Users;
            entity.Store = (Store)stores[entity.Store.Guid];
            entity.User = users[entity.User.Guid.ToString()];
            return base.InsertOneAsync(entity);
        }
    }
}
