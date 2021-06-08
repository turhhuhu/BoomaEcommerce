
using AutoMapper;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Stores;
using BoomaEcommerce.Tests.CoreLib;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;


namespace BoomaEcommerce.Services.Tests
{
    public class RemoveOwnerParallelTests
    {
        private readonly Mock<ILogger<StoresService>> _loggerMock = new();
        private readonly IMapper _mapper = MapperFactory.GetMapper();

        private StoresService GetStoreService(
            IDictionary<Guid, Store> stores,
            IDictionary<Guid, StoreOwnership> storeOwnerships,
            IDictionary<Guid, StorePurchase> storePurchases,
            IDictionary<Guid, StoreManagement> storeManagements,
            IDictionary<Guid, StoreManagementPermissions> storeManagementPermissions,
            IDictionary<Guid, Product> products,
            IDictionary<Guid, User> users)
        {
            var storeUnitOfWork = DalMockFactory.MockStoreUnitOfWork(stores, storeOwnerships, storePurchases,
                storeManagements, storeManagementPermissions, products , null, users);

            return new StoresService(_loggerMock.Object, _mapper, storeUnitOfWork.Object, new NotificationPublisherStub());
        }

        [Theory]
        [Repeat(100)]
        public async Task RemoveStoreOwnerAsync_ReturnTrue_(int iterationNumber)
        {
            // Arrange
            var entitiesOwnerships = new ConcurrentDictionary<Guid, StoreOwnership>();
            var entitiesManagements = new ConcurrentDictionary<Guid, StoreManagement>();
            var entitiesStores = new Dictionary<Guid, Store>();
            var entitiesUsers = new ConcurrentDictionary<Guid, User>();
            var user1 = new User
            {
                Guid = Guid.NewGuid()
            };

            var user2 = new User
            {
                Guid = Guid.NewGuid()
            };

            var userToNominate = new User
            {
                Guid = Guid.NewGuid()
            };
            var store = new Store {Guid = Guid.NewGuid()};
            var firstStoreOwner = new StoreOwnership
                {Store = store, User = user1};
            var secondStoreOwner = new StoreOwnership
                {Store = store, User = user2};
            entitiesUsers[user1.Guid] = user1;
            entitiesUsers[user2.Guid] = user2;
            entitiesUsers[userToNominate.Guid] = userToNominate;
            entitiesOwnerships[firstStoreOwner.Guid] = firstStoreOwner;
            entitiesOwnerships[secondStoreOwner.Guid] = secondStoreOwner;
            entitiesStores[store.Guid] = store;

            var storeManagementDto = new StoreManagementDto
            {
                Store = new StoreDto {Guid = store.Guid},
                User = new UserDto { Guid = userToNominate.Guid },
                Permissions = new StoreManagementPermissionsDto()
            };

            var sut = GetStoreService(entitiesStores, entitiesOwnerships, null, entitiesManagements, null, null, entitiesUsers);

            await sut.NominateNewStoreOwnerAsync(secondStoreOwner.Guid,
                _mapper.Map<StoreOwnershipDto>(secondStoreOwner));

            var taskList = new List<Task<bool>>
            {
                Task.Run((() => sut.NominateNewStoreManagerAsync(secondStoreOwner.Guid, storeManagementDto))),
                Task.Run((() => sut.RemoveStoreOwnerAsync(firstStoreOwner.Guid, secondStoreOwner.Guid)))
            };

            //Act
            var result = await Task.WhenAll(taskList);

            result.Should().Contain(true);
        }
    }
}