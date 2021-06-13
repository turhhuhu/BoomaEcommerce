using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Domain.Discounts;
using BoomaEcommerce.Domain.Policies;
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
    public class PurchaseServiceTests
    {

        private readonly Mock<ILogger<PurchasesService>> _loggerMock = new();
        private readonly Mock<IPaymentClient> _paymentClientMock = new();
        private readonly IMapper _mapper = MapperFactory.GetMapper();
        private readonly IFixture _fixture = new Fixture();
        private readonly Mock<ISupplyClient> _supplyClientMock = new();
        
        public PurchaseServiceTests()
        {
            _fixture.Customize<PurchaseDto>(p => p
                .With(x => x.StorePurchases, TestData.GetTestValidStorePurchasesDtos())
                .With(x => x.TotalPrice, 450)
                .With(x => x.DiscountedPrice, 450)
                .Without(p => p.Buyer)
                .Without(x => x.Guid));
        }

        private PurchasesService GetPurchaseService(
            IDictionary<Guid, Purchase> purchases,
            IDictionary<Guid, Product> products,
            IDictionary<Guid, User> users,
            IDictionary<Guid, ShoppingCart> shoppingCarts,
            IDictionary<Guid, Store> stores)
        {
            var purchaseUnitOfWorkMock = DalMockFactory.MockPurchasesUnitOfWork(purchases, products, users, shoppingCarts
                , new ConcurrentDictionary<Guid, StoreOwnership>(), new ConcurrentDictionary<Guid, Notification>(), stores);
            return new PurchasesService(_mapper, _loggerMock.Object, _paymentClientMock.Object,
                purchaseUnitOfWorkMock.Object, _supplyClientMock.Object, Mock.Of<INotificationPublisher>());
        }

        [Fact]
        public async Task CreatePurchaseAsync_ReturnsTrueAndShouldDecreaseProductsAmount_WhenPurchaseDtoIsValid()
        {
            // Arrange
            var purchasesDict = new Dictionary<Guid, Purchase>();
            var productDict = new Dictionary<Guid, Product>();
            var userDict = new Dictionary<Guid, User>();
            var shoppingCartDict = new Dictionary<Guid, ShoppingCart>();
            var storesDict = new Dictionary<Guid, Store>();

            var purchaseDtoFixture = _fixture.Create<PurchaseDto>();
            
            var purchaseDetailsFixture = _fixture.Build<PurchaseDetailsDto>()
                .With(pd => pd.Purchase, purchaseDtoFixture)
                .Create();

            var userFixture = _fixture.Build<User>()
                .With(x => x.Guid, purchaseDtoFixture.UserBuyerGuid)
                .Create();

            userDict[purchaseDtoFixture.UserBuyerGuid.Value] = userFixture;

            var shoppingCartGuid = Guid.NewGuid();
            var cart = new ShoppingCart(userFixture) {Guid = shoppingCartGuid};
            shoppingCartDict[shoppingCartGuid] = cart;
            
            foreach (var storePurchaseDto in purchaseDtoFixture.StorePurchases)
            {
                foreach (var productsPurchaseDto in storePurchaseDto.PurchaseProducts)
                {
                    var testProductGuid = productsPurchaseDto.ProductGuid;
                    var testProduct = TestData.GetTestProduct(testProductGuid);
                    productDict[testProductGuid] = testProduct;
                    storesDict[storePurchaseDto.StoreGuid] = new Store(new User(), Policy.Empty, Discount.Empty) { Guid = storePurchaseDto.StoreGuid };
                }
            }

            var sut = GetPurchaseService(purchasesDict, productDict, userDict, shoppingCartDict, storesDict);
            
            // Act
            var res = await sut.CreatePurchaseAsync(purchaseDetailsFixture);

            // Assert
            res.Should().NotBeNull();
            foreach (var productDictValue in productDict.Values)
            {
                productDictValue.Amount.Should().Be(5);
            }
            purchasesDict.Values.Should().NotBeEmpty();
            var purchase = purchasesDict.Values.First();
            purchase.Should().BeEquivalentTo(purchaseDtoFixture, 
                opt => opt
                    .Excluding(p => p.Guid)
                    .Excluding(p => p.StorePurchases)
                    .Excluding(p => p.UserBuyerGuid)
                    .Excluding(p => p.Buyer));
        }
        
        [Fact]
        public async Task CreatePurchaseAsync_ReturnsFalseAndShouldNotCreatePurchase_WhenPurchaseDtoIsInvalid()
        {
            // Arrange
            var purchasesDict = new Dictionary<Guid, Purchase>();
            var productDict = new Dictionary<Guid, Product>();
            var userDict = new Dictionary<Guid, User>();
            var shoppingCartDict = new Dictionary<Guid, ShoppingCart>();

            var purchaseDtoFixture = _fixture.Build<PurchaseDto>()
                .With(x => x.StorePurchases, TestData.GetTestInvalidStorePurchasesDtos())
                .With(x => x.TotalPrice, 450)
                .Without(x => x.Guid)
                .Create();

            var purchaseDetailsFixture = _fixture.Build<PurchaseDetailsDto>()
                .With(pd => pd.Purchase, purchaseDtoFixture)
                .Create();

            var userFixture = _fixture.Build<User>()
                .With(x => x.Guid, purchaseDtoFixture.UserBuyerGuid)
                .Create();
            userDict[purchaseDtoFixture.UserBuyerGuid!.Value] = userFixture;
            
            var shoppingCartGuid = Guid.NewGuid();
            var cart = new ShoppingCart(userFixture);
            shoppingCartDict[shoppingCartGuid] = cart;
            
            foreach (var storePurchaseDto in purchaseDtoFixture.StorePurchases)
            {
                foreach (var productsPurchaseDto in storePurchaseDto.PurchaseProducts)
                {
                    var testProductGuid = productsPurchaseDto.ProductGuid;
                    var testProduct = TestData.GetTestProduct(testProductGuid);
                    productDict[testProductGuid] = testProduct;
                }
            }

            var sut = GetPurchaseService(purchasesDict, productDict, userDict, shoppingCartDict, null);
            
            // Act
            var res = await sut.CreatePurchaseAsync(purchaseDetailsFixture);

            // Assert
            res.Should().BeNull();
            foreach (var productDictValue in productDict.Values)
            {
                productDictValue.Amount.Should().Be(10);
            }
            purchasesDict.Values.Should().BeEmpty();
        }

        [Fact]
        public async void GetAllUserPurchaseHistoryAsync_returnEmptyList_userHaveNotBoughtAnything()
        {
            // Arrange
            var entitiesPurchases = new Dictionary<Guid, Purchase>();
            var productDict = new Dictionary<Guid, Product>();
            var userDict = new Dictionary<Guid, User>();

           
            var matanUser = TestData.CreateUserObject("Matan");

            var sut = GetPurchaseService(entitiesPurchases, productDict, userDict, null, null);
            
            //Act
            var result = await sut.GetAllUserPurchaseHistoryAsync(matanUser.Guid);
            
            //Assert
            result.Should().BeEmpty();
        }
        
        [Fact]
        public async void GetAllUserPurchaseHistoryAsync_ReturnPurchaseList_WhenUserHaveMadeTwoPurchases()
        {
            // Arrange
            var entitiesPurchases = new Dictionary<Guid, Purchase>();
            var productDict = new Dictionary<Guid, Product>();
            var userDict = new Dictionary<Guid, User>();

           
            var matanUser = TestData.CreateUserObject("Matan");
            var nikeStore = TestData.CreateStoreObject("nike");
            var shoeProduct = TestData.CreateProductObject("air jordan");
            var shoePurchaseProduct = TestData.CreatePurchaseProductObject(shoeProduct);
            var matanNikeShoeStorePurchase = TestData.CreateStorePurchaseObject(nikeStore, matanUser, shoePurchaseProduct);
            var matanNikeShoePurchase = TestData.CreatePurchaseObject(matanNikeShoeStorePurchase, matanUser);

            var adidasStore = TestData.CreateStoreObject("adidas");
            var shirtProduct = TestData.CreateProductObject(" adidas shirt");
            var shirtPurchaseProduct = TestData.CreatePurchaseProductObject(shirtProduct);
            var matanAdidasShirtStorePurchase = TestData.CreateStorePurchaseObject(adidasStore, matanUser, shirtPurchaseProduct);
            var matanAdidasShirtPurchase = TestData.CreatePurchaseObject(matanAdidasShirtStorePurchase, matanUser);

            entitiesPurchases[matanNikeShoePurchase.Guid] = matanNikeShoePurchase;
            entitiesPurchases[matanAdidasShirtPurchase.Guid] = matanAdidasShirtPurchase;
            
            var sut = GetPurchaseService(entitiesPurchases, productDict, userDict, null, null);
            
            //Act
            var result = await sut.GetAllUserPurchaseHistoryAsync(matanUser.Guid);
            
            //Assert
            result.Should().NotBeEmpty();
            result.Count.Should().Be(2);


        }

    }
}