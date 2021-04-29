using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using BoomaEcommerce.Services.Authentication;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Purchases;
using BoomaEcommerce.Services.Stores;
using BoomaEcommerce.Services.Users;
using BoomaEcommerce.Tests.CoreLib;
using FluentAssertions;
using Xunit;

namespace BoomaEcommerce.AcceptanceTests
{
    public class UserAcceptanceTests : IAsyncLifetime
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


        public async Task InitializeAsync()
        {
            _fixture = new Fixture();
            

           
            var serviceMockFactory = new ServiceMockFactory();

            var storeService = serviceMockFactory.MockStoreService();
            var authService = serviceMockFactory.MockAuthenticationService();
            var purchasesService = serviceMockFactory.MockPurchaseService();
            var usersService = serviceMockFactory.MockUserService();

            await InitUser(storeService, authService, purchasesService ,usersService);
            _fixture.Customize<StoreDto>(s =>
                                         s.Without(ss => ss.Guid).Without(ss => ss.Rating).With(ss => ss.FounderUserGuid , UserGuid));


            

            await InitPurchase(storeService);
        }

        private async Task InitPurchase(IStoresService storesService)
        {

            var store = _fixture.Create<StoreDto>();
            _fixture.Customize<PurchaseDto>(p => p.Without(pp => pp.Guid).With(pp => pp.BuyerGuid, UserGuid));

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
            var purchase_product2 = _fixture.Build<PurchaseProductDto>()
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
                                   .With(p => p.BuyerGuid, UserGuid)
                                   .With(p => p.StorePurchases, store_purchase_lst)
                                   .Without(p => p.Guid)
                                   .With(p => p.TotalPrice, storePurchase.TotalPrice)
                                   .Create();
        }

        private async Task InitUser(IStoresService storeService, IAuthenticationService authService, IPurchasesService purchasesService, IUsersService usersService)
        {
            const string username = "Omer";
            const string password = "Omer1001";

            await authService.RegisterAsync(username, password);
            var loginResponse = await authService.LoginAsync(username, password);

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


        }


        [Fact]
        public async Task ViewPurchasesHistory_ShouldReturnPurchases_WhenPurchasesHistoryExists()
        {
            //Arrange 
            await _purchaseService.CreatePurchaseAsync(purchase);
        
            //Act
            var res = await _purchaseService.GetAllUserPurchaseHistoryAsync(UserGuid);

            //Assert 
            res.First().Should().BeEquivalentTo(purchase, x => x.Excluding(p => p.Guid)
                                                                .Excluding(p=> p.StorePurchases[0].PurchaseProducts[0].ProductGuid)
                                                                .Excluding(p => p.StorePurchases[0].PurchaseProducts[1].ProductGuid)
                                                                .Excluding(p=> p.StorePurchases[0].Guid)
                                                                .Excluding(p=> p.StorePurchases[0].PurchaseProducts[0].Guid)
                                                                .Excluding(p => p.StorePurchases[0].PurchaseProducts[1].Guid));;

        }

        [Fact]
        public async Task CreateStore_ShouldAddStore_WhenStoreDetailsAreValid()
        {
            //Arrange
            var fixtureStore = _fixture
                .Build<StoreDto>()
                .With(s => s.FounderUserGuid, UserGuid )
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
        public async Task AddPurchaseProductToShoppingBasketAsync_ShouldReturnTrueAndAddPurchaseProduct_WhenDetalisAreValid()
        {
            var shoppingCart = await _usersService.GetShoppingCartAsync(UserGuid);

            var fixtureShoppingBasket = _fixture
                .Build<ShoppingBasketDto>()
                .With(s => s.StoreGuid, _store_withGuid.Guid)
                .With(s => s.PurchaseProduct , purchase_product_lst)
                .Without(s => s.Guid)
                .Create();

            var shoppingBasket = await _usersService.CreateShoppingBasketAsync(shoppingCart.Guid, fixtureShoppingBasket);

            var res = await _usersService.AddPurchaseProductToShoppingBasketAsync(shoppingBasket.Guid, purchase_product1);
            var shoppingCart1 = await _usersService.GetShoppingCartAsync(UserGuid);
            var basket = shoppingCart1.Baskets.First();
            basket.PurchaseProduct.First().Should().BeEquivalentTo(purchase_product1, x => x.Excluding(y => y.Guid));
            res.Should().NotBeNull();
    
        }


        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }



    }
}
