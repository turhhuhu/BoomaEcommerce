using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.External;
using BoomaEcommerce.Services.Purchases;
using BoomaEcommerce.Services.Stores;
using BoomaEcommerce.Tests.CoreLib;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BoomaEcommerce.Services.Tests
{
    public class DeleteProductAndPurchaseProductParallelTests
    {
        private readonly Mock<ILogger<StoresService>> _storeLoggerMock = new();
        private readonly Mock<ILogger<PurchasesService>> _purchaseLoggerMock = new();
        private readonly Mock<IPaymentClient> _paymentClientMock = new();
        private readonly IMapper _mapper = MapperFactory.GetMapper();
        private readonly IFixture _fixture = new Fixture();
        private readonly Mock<ISupplyClient> _supplyClientMock = new();

        private StoresService GetStoreService( 
            IDictionary<Guid, Store> stores,
            IDictionary<Guid, StoreOwnership> storeOwnerships,
            IDictionary<Guid, StorePurchase> storePurchases,
            IDictionary<Guid, StoreManagement> storeManagements,
            IDictionary<Guid, StoreManagementPermission> storeManagementPermissions,
            IDictionary<Guid, Product> products)
        {
            var storeUnitOfWork = DalMockFactory.MockStoreUnitOfWork(stores, storeOwnerships, storePurchases,
                storeManagements, storeManagementPermissions, products);
            
            return new StoresService(_storeLoggerMock.Object, _mapper, storeUnitOfWork.Object);
        }

        private PurchasesService GetPurchaseService(
            IDictionary<Guid, Purchase> purchases,
            IDictionary<Guid, Product> products,
            IDictionary<Guid, User> users,
            IDictionary<Guid, ShoppingCart> shoppingCarts)
        {
            var purchaseUnitOfWorkMock = DalMockFactory.MockPurchasesUnitOfWork(purchases, products, users, shoppingCarts);
            return new PurchasesService(_mapper, _purchaseLoggerMock.Object, _paymentClientMock.Object,
                purchaseUnitOfWorkMock.Object, _supplyClientMock.Object);
        }

        [Fact]
        public async Task DeleteProductAsyncAndCreatePurchaseAsync_ReturnsTrueForOneAndFalseForOther_WhenProductIsPurchasedAndDeletedInParallel()
        {
            // Arrange
            var productsDict = new ConcurrentDictionary<Guid, Product>();
            var productGuid = Guid.NewGuid();
            var product = new Product {Guid = productGuid};
            productsDict[productGuid] = product;
            var storeService = GetStoreService(null, null,
                null, null, null, productsDict);
            var purchaseDto = new PurchaseDto
            {
                StorePurchases = new List<StorePurchaseDto>
                {
                    new()
                    {
                        PurchaseProducts = new List<PurchaseProductDto>
                        {
                            new()
                            {
                                ProductGuid = productGuid
                            }
                        }
                    }
                }
            };
            var purchaseService = GetPurchaseService(null, productsDict, null, null);
            var taskList = new List<Task<bool>>
            {
                storeService.DeleteProductAsync(productGuid),
                purchaseService.CreatePurchaseAsync(purchaseDto)
            };
            // Act
            var result = await Task.WhenAll(taskList);
            // Assert
            result.Should().Contain(true).And.Contain(false);
        }
        
    }
}