using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Domain;

namespace BoomaEcommerce.Data.InMemory
{
    public class InMemoryOwnershipRepository : InMemoryRepository<StoreOwnership>
    {
        public override Task InsertOneAsync(StoreOwnership entity)
        {
            if (!RepoContainer.AllEntities.ContainsKey(typeof(Store)))
            {
                RepoContainer.AllEntities.TryAdd(typeof(Store), new Dictionary<Guid, Store>().ToDictionary(x => x.Key, x => (BaseEntity)x.Value));

            }

            InMemoryUserStore.Users ??= new Dictionary<string, User>();
            var stores = RepoContainer.AllEntities[typeof(Store)];
            var users = InMemoryUserStore.Users;
            stores.TryGetValue(entity.Store.Guid, out var store);
            entity.Store = (Store)store;
            users.TryGetValue(entity.User.Guid.ToString(), out var user);
            entity.User = user;
            return base.InsertOneAsync(entity);
        }

        public override async Task DeleteByIdAsync(Guid guid)
        {
            var ownership = await FindByIdAsync(guid);
            var (owners, managers) = ownership.GetSubordinates();
            var managementsRepo = RepoContainer.AllEntities[typeof(StoreManagement)];
            var ownersRepo = RepoContainer.AllEntities[typeof(StoreOwnership)];

            foreach (var storeManagement in managers)
            {
                managementsRepo.Remove(storeManagement.Guid);
            }

            foreach (var storeOwnership in owners)
            {
                ownersRepo.Remove(storeOwnership.Guid);
            }
            await base.DeleteByIdAsync(guid);
        }
    }
    public class InMemoryManagementRepository : InMemoryRepository<StoreManagement>
    {

        public override Task InsertOneAsync(StoreManagement entity)
        {
            if (!RepoContainer.AllEntities.ContainsKey(typeof(Store)))
            {
                RepoContainer.AllEntities.TryAdd(typeof(Store), new Dictionary<Guid, Store>().ToDictionary(x => x.Key, x => (BaseEntity)x.Value));
            }

            InMemoryUserStore.Users ??= new Dictionary<string, User>();

            var stores = RepoContainer.AllEntities[typeof(Store)];
            var users = InMemoryUserStore.Users;
            stores.TryGetValue(entity.Store.Guid, out var store);
            entity.Store = (Store) store;
            users.TryGetValue(entity.User.Guid.ToString(), out var user);
            entity.User = user;
            return base.InsertOneAsync(entity);
        }
    }
}
