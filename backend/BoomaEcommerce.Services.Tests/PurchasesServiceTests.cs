using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using BoomaEcommerce.Data;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.External;
using BoomaEcommerce.Services.MappingProfiles;
using BoomaEcommerce.Services.Purchases;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using ILogger = Castle.Core.Logging.ILogger;

namespace BoomaEcommerce.Services.Tests
{
    public class PurchaseServiceTests
    {

        private readonly Mock<ILogger<PurchasesService>> _loggerMock = new();
        private readonly Mock<IPaymentClient> _paymentClientMock = new();
        private readonly IMapper _mapper = MapperFactory.GetMapper();
        private readonly IFixture _fixture = new Fixture();
        
        
        public Mock<ILogger<PurchasesService>> loggerMock = new Mock<ILogger<PurchasesService>>();

        static MapperConfiguration config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new DomainToDtoProfile());
            cfg.AddProfile(new DtoToDomainProfile());
        });
        

        public PurchaseServiceTests()
        {
            
        }

        private Mock<IPurchaseUnitOfWork> SetUpRepositoriesMocks(IDictionary<Guid, Purchase> purchases, IDictionary<Guid, Product> products,
            IDictionary<Guid, User> users)
        {
            var purchaseRepoMock = DalMockFactory.MockRepository(purchases);
            var productRepoMock = DalMockFactory.MockRepository(products);
            var userRepoMock = DalMockFactory.MockUserManager(users.Values.ToList());
            var purchaseUnitOfWork = new Mock<IPurchaseUnitOfWork>();
            purchaseUnitOfWork.SetupGet(x => x.PurchaseRepository).Returns(purchaseRepoMock.Object);
            purchaseUnitOfWork.SetupGet(x => x.ProductRepository).Returns(productRepoMock.Object);
            purchaseUnitOfWork.SetupGet(x => x.UserRepository).Returns(userRepoMock.Object);
            return purchaseUnitOfWork;
        }

        [Fact]
        public async Task CreatePurchaseAsync_ShouldDecreaseProductsAmount_WhenPurchaseDtoIsValid()
        {
            // Arrange
            var purchasesDict = new Dictionary<Guid, Purchase>();
            var productDict = new Dictionary<Guid, Product>();
            var userDict = new Dictionary<Guid, User>();

            var supplyClientMock = new Mock<ISupplyClient>();
            
            var purchaseDtoFixture = _fixture.Build<PurchaseDto>()
                .With(x => x.StorePurchases, TestData.GetTestValidStorePurchasesDtos())
                .Create();
            
            var userFixture = _fixture.Build<User>()
                .With(x => x.Guid, purchaseDtoFixture.Buyer.Guid)
                .Create();

            userDict[purchaseDtoFixture.Buyer.Guid] = userFixture;
            
            foreach (var storePurchaseDto in purchaseDtoFixture.StorePurchases)
            {
                foreach (var productsPurchaseDto in storePurchaseDto.ProductsPurchases)
                {
                    var testProductGuid = productsPurchaseDto.ProductDto.Guid;
                    var testProduct = TestData.GetTestProduct(testProductGuid);
                    productDict[testProductGuid] = testProduct;
                }
            }

            var purchaseUnitOfWorkMock = SetUpRepositoriesMocks(purchasesDict, productDict, userDict);
        
            var sut = new PurchasesService(_mapper, _loggerMock.Object,
                _paymentClientMock.Object, purchaseUnitOfWorkMock.Object, supplyClientMock.Object);
            
            // Act
            await sut.CreatePurchaseAsync(purchaseDtoFixture);

            // Assert
            foreach (var productDictValue in productDict.Values)
            {
                productDictValue.Amount.Should().Be(5);
            }

            purchasesDict[purchaseDtoFixture.Guid].Guid.Should().Be(purchaseDtoFixture.Guid);
        }
        
        [Fact]
        public async Task CreatePurchaseAsync_ShouldNotAddCreatePurchase_WhenPurchaseDtoIsInvalid()
        {
            // Arrange
            var purchasesDict = new Dictionary<Guid, Purchase>();
            var productDict = new Dictionary<Guid, Product>();
            var userDict = new Dictionary<Guid, User>();
            var supplyClientMock = new Mock<ISupplyClient>();

            var purchaseDtoFixture = _fixture.Build<PurchaseDto>()
                .With(x => x.StorePurchases, TestData.GetTestInvalidStorePurchasesDtos())
                .Create();
            
            var userFixture = _fixture.Build<User>()
                .With(x => x.Guid, purchaseDtoFixture.Buyer.Guid)
                .Create();
            userDict[purchaseDtoFixture.Buyer.Guid] = userFixture;
            
            foreach (var storePurchaseDto in purchaseDtoFixture.StorePurchases)
            {
                foreach (var productsPurchaseDto in storePurchaseDto.ProductsPurchases)
                {
                    var testProductGuid = productsPurchaseDto.ProductDto.Guid;
                    var testProduct = TestData.GetTestProduct(testProductGuid);
                    productDict[testProductGuid] = testProduct;
                }
            }

            var purchaseUnitOfWorkMock = SetUpRepositoriesMocks(purchasesDict, productDict, userDict);
        
            var sut = new PurchasesService(_mapper, _loggerMock.Object,
                _paymentClientMock.Object, purchaseUnitOfWorkMock.Object, supplyClientMock.Object);
            
            // Act
            await sut.CreatePurchaseAsync(purchaseDtoFixture);

            // Assert
            foreach (var productDictValue in productDict.Values)
            {
                productDictValue.Amount.Should().Be(10);
            }

            productDict.ContainsKey(purchaseDtoFixture.Guid).Should().BeFalse();
        }

        [Fact]
        public async void GetAllUserPurchaseHistoryAsync_returnEmptyList_userHaveNotBoughtAnything()
        {
            // Arrange
            var mapper = config.CreateMapper();

            var entitiesPurchases = new Dictionary<Guid, Purchase>();
            var productDict = new Dictionary<Guid, Product>();
            var userDict = new Dictionary<Guid, User>();

           
            var matanUser = createUserObject("Matan");

            /*var nikeStore = createStoreObject("nike");

            var bennyUser = createUserObject("Benny");
            
            var storeOwnershipMatanNike = createStoreOwnershipObject(nikeStore, matanUser);*/
            

            //EntitiesOwnerships[storeOwnershipMatanNike.Guid] = storeOwnershipMatanNike;


            var purchasesUnitOfWork = DalMockFactory.MockPurchasesUnitOfWork(entitiesPurchases, productDict, userDict);

            var ps = new PurchasesService(mapper, loggerMock.Object, null, purchasesUnitOfWork.Object, null);
            
            //Act
            var result = await ps.GetAllUserPurchaseHistoryAsync(matanUser.Guid);
            
            //Assert
            result.Should().BeEmpty();
        }
        
        [Fact]
        public async void GetAllUserPurchaseHistoryAsync_returnPurchaseList_userHaveMadeTwoPurchases()
        {
            // Arrange
            var mapper = config.CreateMapper();

            var entitiesPurchases = new Dictionary<Guid, Purchase>();
            var productDict = new Dictionary<Guid, Product>();
            var userDict = new Dictionary<Guid, User>();

           
            var matanUser = createUserObject("Matan");
            var nikeStore = createStoreObject("nike");
            var shoeProduct = createProductObject("air jordan");
            var shoePurchaseProduct = createPurchaseProductObject(shoeProduct);
            var matanNikeShoeStorePurchase = createStorePurchaseObject(nikeStore, matanUser, shoePurchaseProduct);
            var matanNikeShoePurchase = createPurchaseObject(matanNikeShoeStorePurchase, matanUser);

            var adidasStore = createStoreObject("adidas");
            var shirtProduct = createProductObject(" adidas shirt");
            var shirtPurchaseProduct = createPurchaseProductObject(shirtProduct);
            var matanAdidasShirtStorePurchase = createStorePurchaseObject(adidasStore, matanUser, shirtPurchaseProduct);
            var matanAdidasShirtPurchase = createPurchaseObject(matanAdidasShirtStorePurchase, matanUser);

            entitiesPurchases[matanNikeShoePurchase.Guid] = matanNikeShoePurchase;
            entitiesPurchases[matanAdidasShirtPurchase.Guid] = matanAdidasShirtPurchase;
            

            var purchasesUnitOfWork = DalMockFactory.MockPurchasesUnitOfWork(entitiesPurchases, productDict, userDict);

            var ps = new PurchasesService(mapper, loggerMock.Object, null, purchasesUnitOfWork.Object, null);
            
            //Act
            var result = await ps.GetAllUserPurchaseHistoryAsync(matanUser.Guid);
            
            //Assert
            result.Should().NotBeEmpty();
            result.Count.Should().Be(2);


        }
        private Store createStoreObject(string storeName)
        {
            return new() { StoreName = storeName };
        }

        private User createUserObject(string name)
        {
            return new() { Name = name };
        }

        private Product createProductObject(string name)
        {
            return new() {Name = name};
        }

        private PurchaseProduct createPurchaseProductObject(Product p)
        {
            return new()
            {
                Product = p
            };
        }

        private StorePurchase createStorePurchaseObject(Store store, User user, PurchaseProduct product)
        {
            return new ()
            {
                ProductsPurchases = new List<PurchaseProduct>{product},
                Buyer = user,
                Store = store,
                TotalPrice = 5
                
            };
        }

        private Purchase createPurchaseObject(StorePurchase sp,User buyer)
        {
            return new()
            {
                Buyer = buyer,
                StorePurchases = new List<StorePurchase> {sp}
            };
        }

    }
}