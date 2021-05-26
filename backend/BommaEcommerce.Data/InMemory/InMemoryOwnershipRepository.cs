﻿using System;
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
            var stores = RepoContainer.AllEntities[typeof(Store)];
            var users = InMemoryUserStore.Users;
            entity.Store = (Store)stores[entity.Store.Guid];
            entity.User = users[entity.User.Guid.ToString()];
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
            var stores = RepoContainer.AllEntities[typeof(Store)];
            var users = InMemoryUserStore.Users;
            entity.Store = (Store)stores[entity.Store.Guid];
            entity.User = users[entity.User.Guid.ToString()];
            return base.InsertOneAsync(entity);
        }
    }
}