using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.Authentication;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Purchases;
using BoomaEcommerce.Services.Stores;
using BoomaEcommerce.Services.Users;
using BoomaEcommerce.Tests.CoreLib;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BoomaEcommerce.AcceptanceTests
{
    public class UserAcceptanceTests : TestsBase
    {
        private IStoresService _storeService;
        private IPurchasesService _purchaseService;
        private IUsersService _usersService;
        private IFixture _fixture;
        private Guid UserGuid;
        private PurchaseDto purchase;
        private StoreDto _store_withGuid;
        private List<PurchaseProductDto> purchase_product_lst; 
        private ProductDto product1;
        private ProductDto product2;
        private PurchaseProductDto purchase_product1;
        private PurchaseProductDto purchase_product2;
        private NotificationPublisherStub _notificationPublisher;
        private Guid _founderGuid;


        public override async Task InitInMemoryDb()
        {
            await base.InitInMemoryDb();
            _fixture = new Fixture();

            _founderGuid = Guid.NewGuid();

            var serviceMockFactory = new ServiceMockFactory();

            var storeService = serviceMockFactory.MockStoreService();
            var authService = serviceMockFactory.MockAuthenticationService();
            var purchasesService = serviceMockFactory.MockPurchaseService();
            _notificationPublisher = serviceMockFactory.GetNotificationPublisherStub();
            var usersService = serviceMockFactory.MockUserService();

            await InitUser(storeService, authService, purchasesService, usersService);
            _fixture.Customize<StoreDto>(s => s
                .Without(ss => ss.Guid)
                .Without(ss => ss.Rating)
                .With(ss => ss.FounderUserGuid, _founderGuid));

            await InitPurchase(storeService);
        }

        public override async Task InitEfCoreDb(ServiceProvider provider)
        {
            _fixture = new Fixture();

            _founderGuid = Guid.NewGuid();

            var storeService = provider.GetRequiredService<StoresService>();
            var authService = provider.GetRequiredService<IAuthenticationService>();
            var purchasesService = provider.GetRequiredService<PurchasesService>();
            _notificationPublisher = provider.GetRequiredService<NotificationPublisherStub>();
            var usersService = provider.GetRequiredService<UsersService>();

            await InitUser(storeService, authService, purchasesService, usersService);
            _fixture.Customize<StoreDto>(s => s
                .Without(ss => ss.Guid)
                .Without(ss => ss.Rating)
                .With(ss => ss.FounderUserGuid, _founderGuid));

            await InitPurchase(storeService);
        }

        public UserAcceptanceTests(SharedDatabaseFixture dataBaseFixture) : base(dataBaseFixture)
        {
        }

        private async Task InitPurchase(IStoresService storesService)
        {

            var store = _fixture.Create<StoreDto>();
            _fixture.Customize<PurchaseDto>(p => p.Without(pp => pp.Guid).With(pp => pp.UserBuyerGuid, UserGuid));

            _store_withGuid = await _storeService.CreateStoreAsync(store);
            _fixture.Customize<ProductDto>(p => p.Without(pp => pp.Guid).With(pp => pp.StoreGuid, _store_withGuid.Guid).Without(pp => pp.Rating).With(pp => pp.Price, 2).With(pp => pp.Amount, 1));

            product1 = _fixture.Create<ProductDto>();
            product2 = _fixture.Create<ProductDto>();


            var product1_withGuid = await storesService.CreateStoreProductAsync(product1);
            var product2_withGuid = await storesService.CreateStoreProductAsync(product2);


            purchase_product1 = _fixture.Build<PurchaseProductDto>()
                                            .With(pp => pp.ProductGuid, product1_withGuid.Guid)
                                            .Without(pp => pp.Guid)
                                            .With(pp => pp.Price, product1_withGuid.Price)
                                            .With(pp => pp.Amount, 1)
                                            .Create();

            purchase_product2 = _fixture.Build<PurchaseProductDto>()
                                            .With(pp => pp.ProductGuid, product2_withGuid.Guid)
                                            .With(pp => pp.Price, product2_withGuid.Price)
                                            .With(pp => pp.Amount, 1)
                                            .Without(pp => pp.Guid)
                                            .Create();


            purchase_product_lst = new List<PurchaseProductDto>();

            purchase_product_lst.Add(purchase_product1);
            purchase_product_lst.Add(purchase_product2);

            var storePurchase = _fixture.Build<StorePurchaseDto>()
                                        .With(sp => sp.StoreGuid, _store_withGuid.Guid)
                                        .With(sp => sp.BuyerGuid, UserGuid)
                                        .With(sp => sp.PurchaseProducts, purchase_product_lst)
                                        .Without(sp => sp.Guid)
                                        .With(sp => sp.TotalPrice, purchase_product1.Price + purchase_product2.Price)
                                        .Create();
            var store_purchase_lst = new List<StorePurchaseDto>();
            store_purchase_lst.Add(storePurchase);

            purchase = _fixture.Build<PurchaseDto>()
                                   .With(p => p.UserBuyerGuid, UserGuid)
                                   .With(p => p.StorePurchases, store_purchase_lst)
                                   .Without(p => p.Guid)
                                   .With(p => p.TotalPrice, storePurchase.TotalPrice)
                                   .With(p => p.DiscountedPrice, storePurchase.TotalPrice)
                                   .Create();
        }

        private async Task InitUser(IStoresService storeService, IAuthenticationService authService, IPurchasesService purchasesService, IUsersService usersService)
        {
            var user = new UserDto {UserName = "Omer"};
            const string password = "Omer1001";

            await authService.RegisterAsync(user, password);
            var loginResponse = await authService.LoginAsync(user.UserName, password);

            UserGuid = loginResponse.UserGuid;


            var createStoreServiceResult = SecuredStoreService.CreateSecuredStoreService(loginResponse.Token,
                 ServiceMockFactory.Secret, storeService, out _storeService);
            if (!createStoreServiceResult)
            {
                throw new Exception("This shouldn't happen");
            }

            var createPurchasesServiceResult = SecuredPurchaseService.CreateSecuredPurchaseService(loginResponse.Token,
                 ServiceMockFactory.Secret, purchasesService, out _purchaseService);
            if (!createPurchasesServiceResult)
            {
                throw new Exception("This shouldn't happen");
            }

            var createUsersServiceResult = SecuredUserService.CreateSecuredUserService(loginResponse.Token,
                 ServiceMockFactory.Secret, usersService, out _usersService);
            if (!createUsersServiceResult)
            {
                throw new Exception("This shouldn't happen");
            }

            await _notificationPublisher.addNotifiedUser(loginResponse.UserGuid, new List<NotificationDto>());

        }


        [Fact]
        public async Task ViewPurchasesHistory_ShouldReturnPurchases_WhenPurchasesHistoryExists()
        {
            var purchaseProductDetails = _fixture.Build<PurchaseDetailsDto>()
                .With(pd => pd.Purchase, purchase)
                .Create();
            purchaseProductDetails.Purchase.Buyer = null;
            //Arrange 
            await _purchaseService.CreatePurchaseAsync(purchaseProductDetails);
        
            //Act
            var res = await _purchaseService.GetAllUserPurchaseHistoryAsync(UserGuid);

            //Assert 
            res.First().Should().BeEquivalentTo(purchase, x => x.Excluding(p => p.Guid)
                                                                .Excluding(p=> p.StorePurchases[0].PurchaseProducts[0].ProductGuid)
                                                                .Excluding(p => p.StorePurchases[0].PurchaseProducts[0].ProductMetaData)
                                                                .Excluding(p => p.StorePurchases[0].PurchaseProducts[0].ProductGuid)
                                                                .Excluding(p => p.StorePurchases[0].PurchaseProducts[1].ProductGuid)
                                                                .Excluding(p=> p.StorePurchases[0].Guid)
                                                                .Excluding(p => p.StorePurchases[0].UserMetaData)
                                                                .Excluding(p => p.StorePurchases[0].StoreMetaData)
                                                                .Excluding(p=> p.StorePurchases[0].PurchaseProducts[0].Guid)
                                                                .Excluding(p => p.StorePurchases[0].PurchaseProducts[1].ProductMetaData)
                                                                .Excluding(p => p.StorePurchases[0].PurchaseProducts[1].Guid)
                                                                .Excluding(p =>p.Buyer));

        }

        [Fact]
        public async Task CreateStore_ShouldAddStore_WhenStoreDetailsAreValid()
        {
            //Arrange
            var newFounderGuid = Guid.NewGuid();

            var fixtureStore = _fixture
                .Build<StoreDto>()
                .With(s => s.FounderUserGuid, newFounderGuid)
                .Without(s => s.Rating)
                .Without(s => s.Guid)
                .Create();
            //Act 
            var newStore = await _storeService.CreateStoreAsync(fixtureStore);

            //Assert 
            var expectedStore = await _storeService.GetStoreAsync(newStore.Guid);
            

            expectedStore.Should().BeEquivalentTo(newStore);
        }

        [Fact]
        public async Task AddPurchaseProductToShoppingBasketAsync_ShouldReturnTrueAndAddPurchaseProduct_WhenDetailsAreValid()
        {
            // Arrange
            var shoppingCart = await _usersService.GetShoppingCartAsync(UserGuid);

            var fixtureShoppingBasket = _fixture
                .Build<ShoppingBasketDto>()
                .With(s => s.StoreGuid, _store_withGuid.Guid)
                .With(s => s.PurchaseProducts , purchase_product_lst)
                .Without(s => s.Guid)
                .Create();

            var shoppingBasket = await _usersService.CreateShoppingBasketAsync(shoppingCart.Guid, fixtureShoppingBasket);

            // Act
            var res = await _usersService.AddPurchaseProductToShoppingBasketAsync(UserGuid, shoppingBasket.Guid, purchase_product1);
            var shoppingCart1 = await _usersService.GetShoppingCartAsync(UserGuid);
            var basket = shoppingCart1.Baskets.First();
            basket.PurchaseProducts.First(x => x.ProductGuid == purchase_product1.ProductGuid).Should().BeEquivalentTo(purchase_product1, x => x.Excluding(y => y.Guid).Excluding(y => y.Amount).Excluding(y => y.ProductMetaData));
            res.Should().NotBeNull();    
        }

        [Fact]
        public async Task GetShoppingCartAsync_ReturnsValidContentOfCart_WhenCartExists()
        {
            // Arrange
            var shoppingCart = await _usersService.GetShoppingCartAsync(UserGuid);

            var fixtureShoppingBasket = _fixture
                .Build<ShoppingBasketDto>()
                .With(s => s.StoreGuid, _store_withGuid.Guid)
                .With(s => s.PurchaseProducts, purchase_product_lst)
                .Without(s => s.Guid)
                .Create();

            var shoppingBasket = await _usersService.CreateShoppingBasketAsync(shoppingCart.Guid, fixtureShoppingBasket);
            var res = await _usersService.AddPurchaseProductToShoppingBasketAsync(UserGuid, shoppingBasket.Guid, purchase_product1);

            // Act
            var shoppingCart1 = await _usersService.GetShoppingCartAsync(UserGuid);

            shoppingCart1.Baskets.Count.Should().Be(1);

        }

        [Fact]
        public async Task GetShoppingCartAsync_ReturnsEmptyCart_WhenCartDoesNotExist()
        {
            // Act
            var shoppingCart = await _usersService.GetShoppingCartAsync(UserGuid);
            
            // Assert
            shoppingCart.Baskets.Count.Should().Be(0);

        }

        [Fact]
        public async Task PurchaseProduct_ShouldPerformPurchase_WhenPurchaseDetailsAreValid()
        {
            //Arrange
            var myStorePurchase = _fixture.Build<StorePurchaseDto>()
                .With(storePurchaseDto => storePurchaseDto.StoreGuid, _store_withGuid.Guid)
                .With(storePurchaseDto => storePurchaseDto.BuyerGuid, UserGuid)
                .With(storePurchaseDto => storePurchaseDto.PurchaseProducts, purchase_product_lst)
                .Without(storePurchaseDto => storePurchaseDto.Guid)
                .With(storePurchaseDto => storePurchaseDto.TotalPrice, purchase_product1.Price + purchase_product2.Price)
                .Create();

            List<StorePurchaseDto> sp = new List<StorePurchaseDto>();
            sp.Add(myStorePurchase);

            var myPurchase = _fixture.Build<PurchaseDto>()
                .With(p => p.StorePurchases, sp)
                .With(p => p.TotalPrice, purchase_product1.Price + purchase_product2.Price)
                .With(p => p.DiscountedPrice, purchase_product1.Price + purchase_product2.Price)
                .With(p => p.UserBuyerGuid, UserGuid)
                .Without(p => p.Guid)
                .Without(p => p.Buyer)
                .Create();
            
            var purchaseProductDetails = _fixture.Build<PurchaseDetailsDto>()
                .With(pd => pd.Purchase, myPurchase)
                .Create();

            // Act 

            var purchaseWasSuccessful = await _purchaseService.CreatePurchaseAsync(purchaseProductDetails);

            // Assert
            purchaseWasSuccessful.Should().NotBeNull();
        }

        [Fact]
        public async Task PurchaseProduct_ShouldNotPerformPurchase_WhenPurchaseDetailsAreInvalid()
        {
            //Arrange
            var myStorePurchase = _fixture.Build<StorePurchaseDto>()
                .With(sp => sp.StoreGuid, _store_withGuid.Guid)
                .With(sp => sp.BuyerGuid, UserGuid)
                .With(sp => sp.PurchaseProducts, purchase_product_lst)
                .Without(sp => sp.Guid)
                .With(sp => sp.TotalPrice, purchase_product1.Price + purchase_product2.Price)
                .Create();

            List<StorePurchaseDto> sp = new List<StorePurchaseDto>();
            sp.Add(myStorePurchase);


            var myPurchase = _fixture.Build<PurchaseDto>()
                .With(p => p.StorePurchases, sp)
                .With(p => p.TotalPrice, purchase_product1.Price + purchase_product2.Price + 1)
                .With(p => p.UserBuyerGuid, UserGuid)
                .Without(p => p.Guid)
                .Without(p => p.Buyer)
                .Create();

            var purchaseProductDetails = _fixture.Build<PurchaseDetailsDto>()
                .With(pd => pd.Purchase, myPurchase)
                .Create();
           

            // Act 
            var purchaseWasSuccessful = await _purchaseService.CreatePurchaseAsync(purchaseProductDetails);

            // Assert
            purchaseWasSuccessful.Should().BeNull();
        }

        [Fact]
        public async Task DeletePurchaseProductFromShoppingBasketAsync_UpdatesContentOfCart_WhenDetailsAreValid()
        {
            // Arrange
            var shoppingCart = await _usersService.GetShoppingCartAsync(UserGuid);

            var fixtureShoppingBasket = _fixture
                .Build<ShoppingBasketDto>()
                .With(s => s.StoreGuid, _store_withGuid.Guid)
                .With(s => s.PurchaseProducts, purchase_product_lst)
                .Without(s => s.Guid)
                .Create();

            var shoppingBasket = await _usersService.CreateShoppingBasketAsync(shoppingCart.Guid, fixtureShoppingBasket);
            var guidToDelete = ((await _usersService.GetShoppingCartAsync(UserGuid)).Baskets.First())
                .PurchaseProducts[0].Guid;

            // Act
            var success =
                await _usersService.DeletePurchaseProductFromShoppingBasketAsync(shoppingBasket.Guid,
                    guidToDelete);
            var shoppingCartUpdated = await _usersService.GetShoppingCartAsync(UserGuid);
            var list = shoppingCartUpdated.Baskets.First().PurchaseProducts;


            // Assert
            list.FirstOrDefault(p => p.Guid == guidToDelete).Should().BeNull();
            success.Should().BeTrue();

        }

        [Fact]
        public async Task DeletePurchaseProductFromShoppingBasketAsync_UpdatesContentOfCartDoesNotHappen_WhenDetailsAreNotValid()
        {
            // Arrange
            var shoppingCart = await _usersService.GetShoppingCartAsync(UserGuid);
            var fixtureShoppingBasket = _fixture
                .Build<ShoppingBasketDto>()
                .With(s => s.StoreGuid, _store_withGuid.Guid)
                .With(s => s.PurchaseProducts, purchase_product_lst)
                .Without(s => s.Guid)
                .Create();

            var shoppingBasket = await _usersService.CreateShoppingBasketAsync(shoppingCart.Guid, fixtureShoppingBasket);
            var guidToDelete = ((await _usersService.GetShoppingCartAsync(UserGuid)).Baskets.First())
                .PurchaseProducts[0].Guid;

            // Act
            var success =
                await _usersService.DeletePurchaseProductFromShoppingBasketAsync(shoppingBasket.Guid,
                    Guid.NewGuid());
            var shoppingCartUpdated = await _usersService.GetShoppingCartAsync(UserGuid);
            var list = shoppingCartUpdated.Baskets.First().PurchaseProducts;

            // Assert
            list.Find(p => p.Guid == guidToDelete).Should().NotBeNull();
            success.Should().BeFalse();

        }

    }
}
