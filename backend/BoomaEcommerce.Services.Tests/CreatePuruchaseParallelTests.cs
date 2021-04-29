using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.External;
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
            IDictionary<Guid, ShoppingCart> shoppingCarts)
        {
            var purchaseUnitOfWorkMock = DalMockFactory.MockPurchasesUnitOfWork(purchases, products, users, shoppingCarts);
            return new PurchasesService(_mapper, _loggerMock.Object, _paymentClientMock.Object,
                purchaseUnitOfWorkMock.Object, _supplyClientMock.Object);
        }

        [Fact]
        public async Task CreatePurchaseAsync_ReturnsTrueForOneAndFalseForOther_WhenTwoCustomersBuyLastProductInParallel()
        {
            // Arrange
            var purchasesDict = new Dictionary<Guid, Purchase>();
            var productDict = new Dictionary<Guid, Product>();
            var userDict = new Dictionary<Guid, User>();
            var shoppingCartDict = new Dictionary<Guid, ShoppingCart>();

            var userGuid = Guid.NewGuid();
            var userFixture = _fixture.Build<User>()
                .With(x => x.Guid, userGuid)
                .Create();
            userDict[userGuid] = userFixture;
            
            var shoppingCartGuid = Guid.NewGuid();
            var cart = new ShoppingCart(userFixture) {Guid = shoppingCartGuid};
            shoppingCartDict[shoppingCartGuid] = cart;

            var productGuid = Guid.NewGuid();
            var product = new Product {Guid = productGuid, Amount = 1, ProductLock = new SemaphoreSlim(1)};
            productDict[productGuid] = product;

            var sut = GetPurchaseService(purchasesDict, productDict, userDict, shoppingCartDict);
            
            // Act
            var taskList = new List<Task<bool>>
            {
                sut.CreatePurchaseAsync(TestData.GetPurchaseWithSingleProductWithAmountOf1(userGuid, productGuid)),
                sut.CreatePurchaseAsync(TestData.GetPurchaseWithSingleProductWithAmountOf1(userGuid, productGuid))
            };
            var res = await Task.WhenAll(taskList);
            
            // Assert
            res.Should().Contain(true).And.Contain(false);
            productDict[productGuid].Amount.Should().Be(0);

        }
        
    }
}