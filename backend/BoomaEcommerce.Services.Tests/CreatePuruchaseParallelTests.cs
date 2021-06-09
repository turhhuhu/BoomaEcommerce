using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.External;
using BoomaEcommerce.Services.External.Payment;
using BoomaEcommerce.Services.External.Supply;
using BoomaEcommerce.Services.Purchases;
using BoomaEcommerce.Tests.CoreLib;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BoomaEcommerce.Services.Tests
{
    public class ParallelTests
    {
        private readonly Mock<ILogger<PurchasesService>> _loggerMock = new();
        private readonly Mock<IPaymentClient> _paymentClientMock = new();
        private readonly IMapper _mapper = MapperFactory.GetMapper();
        private readonly IFixture _fixture = new Fixture();
        private readonly Mock<ISupplyClient> _supplyClientMock = new();
        
        private PurchasesService GetPurchaseService(
            IDictionary<Guid, Purchase> purchases,
            IDictionary<Guid, Product> products,
            IDictionary<Guid, User> users,
            IDictionary<Guid, ShoppingCart> shoppingCarts,
            IDictionary<Guid, Store>  stores)
        {
            var purchaseUnitOfWorkMock = DalMockFactory.MockPurchasesUnitOfWork(purchases, products, users, shoppingCarts,
                new ConcurrentDictionary<Guid, StoreOwnership>(), new ConcurrentDictionary<Guid, Notification>(), stores);
            return new PurchasesService(_mapper, _loggerMock.Object, _paymentClientMock.Object,
                purchaseUnitOfWorkMock.Object, _supplyClientMock.Object, Mock.Of<INotificationPublisher>());
        }

        [Theory]
        [Repeat(100)]
        public async Task CreatePurchaseAsync_ReturnsTrueForOneAndFalseForOther_WhenTwoCustomersBuyLastProductInParallel(int iterationNumber)
        {
            // Arrange
            var purchasesDict = new Dictionary<Guid, Purchase>();
            var productDict = new Dictionary<Guid, Product>();
            var userDict = new Dictionary<Guid, User>();
            var shoppingCartDict = new Dictionary<Guid, ShoppingCart>();
            var storesDict = new Dictionary<Guid, Store>();

            var userGuid = Guid.NewGuid();
            var userFixture = _fixture.Build<User>()
                .With(x => x.Guid, userGuid)
                .Create();
            userDict[userGuid] = userFixture;
            
            var shoppingCartGuid = Guid.NewGuid();
            var cart = new ShoppingCart(userFixture) {Guid = shoppingCartGuid};
            shoppingCartDict[shoppingCartGuid] = cart;

            var store = new Store();
            storesDict[store.Guid] = store;

            var productGuid = Guid.NewGuid();
            var product = new Product {Guid = productGuid, Amount = 1, ProductLock = new SemaphoreSlim(1), Store = store };
            productDict[productGuid] = product;



            var sut = GetPurchaseService(purchasesDict, productDict, userDict, shoppingCartDict, storesDict);
            
            // Act
            var taskList = new List<Task<PurchaseDto>>
            {
                sut.CreatePurchaseAsync(TestData.GetPurchaseDetailsWithPurchaseWithSingleProductWithAmountOf1(userGuid, productGuid, product.Store.Guid)),
                sut.CreatePurchaseAsync(TestData.GetPurchaseDetailsWithPurchaseWithSingleProductWithAmountOf1(userGuid, productGuid, product.Store.Guid))
            };
            var res = await Task.WhenAll(taskList);
            
            // Assert
            res.Should().Contain(x => x == null).And.Contain(x => x != null);
            productDict[productGuid].Amount.Should().Be(0);

        }
        
    }
}