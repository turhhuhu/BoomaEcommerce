using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Stores;
using BoomaEcommerce.Tests.CoreLib;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BoomaEcommerce.Services.Tests
{
    public class NominateNewStoreManagerParallelTests
    {
        private readonly Mock<ILogger<StoresService>> _loggerMock = new();
        private readonly IMapper _mapper = MapperFactory.GetMapper();

        private StoresService GetStoreService( 
            IDictionary<Guid, Store> stores,
            IDictionary<Guid, StoreOwnership> storeOwnerships,
            IDictionary<Guid, StorePurchase> storePurchases,
            IDictionary<Guid, StoreManagement> storeManagements,
            IDictionary<Guid, StoreManagementPermissions> storeManagementPermissions,
            IDictionary<Guid, Product> products)
        {
            var storeUnitOfWork = DalMockFactory.MockStoreUnitOfWork(stores, storeOwnerships, storePurchases,
                storeManagements, storeManagementPermissions, products , null, null);
            
            return new StoresService(_loggerMock.Object, _mapper, storeUnitOfWork.Object, new NotificationPublisherStub());
        }
        
        [Theory]
        [Repeat(100)]
        public async Task NominateNewStoreManager_ReturnTrue_NewManagerDoesNotHaveOtherResponsibilities(int iterationNumber)
        {
            // Arrange
            var entitiesOwnerships = new ConcurrentDictionary<Guid, StoreOwnership>();
            var entitiesManagements = new ConcurrentDictionary<Guid, StoreManagement>();

            var storeGuid = Guid.NewGuid();
            var firstStoreOwner = new StoreOwnership {Store = new Store(null) {Guid = storeGuid}, User = new User{Guid = Guid.NewGuid()}};
            var secondStoreOwner = new StoreOwnership {Store = new Store(null) { Guid = storeGuid}, User = new User{Guid = Guid.NewGuid()}};
            entitiesOwnerships[firstStoreOwner.Guid] = firstStoreOwner;
            entitiesOwnerships[secondStoreOwner.Guid] = secondStoreOwner;

            var storeManagementDto = new StoreManagementDto
            {
                Store = new StoreDto { Guid = storeGuid },
                User = new UserDto { Guid = Guid.NewGuid() },
                Permissions = new StoreManagementPermissionsDto()
            };

            var sut = GetStoreService(null, entitiesOwnerships, null, entitiesManagements, null, null);

            var taskList = new List<Task<bool>>
            {
                sut.NominateNewStoreManagerAsync(firstStoreOwner.Guid, storeManagementDto),
                sut.NominateNewStoreManagerAsync(secondStoreOwner.Guid, storeManagementDto)
            };
            
            //Act
            var result = await Task.WhenAll(taskList);
            
            //Assert
            result.Should().Contain(true).And.Contain(false);
        }
    }
}