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
using BoomaEcommerce.Tests.CoreLib;
using FluentAssertions;
using Xunit;

namespace BoomaEcommerce.AcceptanceTests
{
    public class UserAcceptanceTests : IAsyncLifetime
    {
        private IStoresService _storeService;
        private  IPurchasesService _purchaseService;
        private IFixture _fixture;
        private UserDto user;
        private StoreDto _store_withGuid;
        private ProductDto product1_withGuid;
        private ProductDto product2_withGuid;

        public async Task InitializeAsync()
        {
            _fixture = new Fixture();
            

           
            var serviceMockFactory = new ServiceMockFactory();

            var storeService = serviceMockFactory.MockStoreService();
            var authService = serviceMockFactory.MockAuthenticationService();
            var purchasesService = serviceMockFactory.MockPurchaseService();

            await InitUser(storeService, authService, purchasesService );
            _fixture.Customize<StoreDto>(s =>
                                         s.Without(ss => ss.Guid).Without(ss => ss.Rating).With(ss => ss.StoreFounder , user));


            var store = _fixture.Create<StoreDto>();
            _fixture.Customize<PurchaseDto>(p => p.Without(pp => pp.Guid).With(pp => pp.Buyer, user));

            _store_withGuid = await _storeService.CreateStoreAsync(store);
            _fixture.Customize<StorePurchaseDto>(p => p.Without(pp => pp.Guid).With(pp => pp.Store, _store_withGuid));
            _fixture.Customize<ProductDto>(p => p.Without(pp => pp.Guid).With(pp => pp.Store, _store_withGuid).Without(pp => pp.Rating).With(pp => pp.Price , 2).With(pp => pp.Amount, 1));

            var product1 = _fixture.Create<ProductDto>();
            var product2 = _fixture.Create<ProductDto>();

            product1_withGuid = await storeService.CreateStoreProductAsync(product1);
            product2_withGuid = await storeService.CreateStoreProductAsync(product2);
        }

        private async Task InitUser(IStoresService storeService, IAuthenticationService authService, IPurchasesService purchasesService)
        {
            const string username = "Omer";
            const string password = "Omer1001";

            await authService.RegisterAsync(username, password);
            var loginResponse = await authService.LoginAsync(username, password);

            user = loginResponse.User;


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


        }


        [Fact]
        public async Task ViewPurchasesHistory_ShouldReturnPurchases_WhenPurchasesHistoryExists()
        {
            //Arrange 
            var purchase_product1 = _fixture.Build<PurchaseProductDto>()
                                            .With(pp => pp.Product, product1_withGuid)
                                            .Without(pp => pp.Guid)
                                            .With(pp => pp.Price , product1_withGuid.Price)
                                            .With(pp=> pp.Amount , 1)
                                            .Create();
            var purchase_product2 = _fixture.Build<PurchaseProductDto>()
                                            .With(pp => pp.Product, product2_withGuid)
                                            .With(pp => pp.Price, product2_withGuid.Price)
                                            .With(pp => pp.Amount, 1)
                                            .Without(pp => pp.Guid)
                                            .Create();
            var purchase_product_lst = new List<PurchaseProductDto>();

            purchase_product_lst.Add(purchase_product1);
            purchase_product_lst.Add(purchase_product2);

            var storePurchase = _fixture.Build<StorePurchaseDto>()
                                        .With(sp => sp.Store, _store_withGuid)
                                        .With(sp => sp.Buyer, user)
                                        .With(sp => sp.PurchaseProducts, purchase_product_lst)
                                        .Without(sp => sp.Guid)
                                        .With(sp => sp.TotalPrice , purchase_product1.Price + purchase_product2.Price)
                                        .Create();
            var store_purchase_lst = new List<StorePurchaseDto>();
            store_purchase_lst.Add(storePurchase);

            var purchase = _fixture.Build<PurchaseDto>()
                                   .With(p => p.Buyer, user)
                                   .With(p => p.StorePurchases, store_purchase_lst)
                                   .Without(p => p.Guid)
                                   .With(p => p.TotalPrice , storePurchase.TotalPrice)
                                   .Create();
            
            await _purchaseService.CreatePurchaseAsync(purchase);
        
            //Act
            var res = await _purchaseService.GetAllUserPurchaseHistoryAsync(user.Guid);

            //Assert 
            res.First().Should().BeEquivalentTo(purchase, x => x.Excluding(p => p.Guid)
                                                                .Excluding(p=> p.StorePurchases[0].PurchaseProducts[0].Product.Amount)
                                                                .Excluding(p => p.StorePurchases[0].PurchaseProducts[1].Product.Amount)
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
                .With(s => s.StoreFounder, user )
                .Without(s => s.Rating)
                .Without(s => s.Guid)
                .Create();
            //Act 
            var newStore = await _storeService.CreateStoreAsync(fixtureStore);

            //Assert 
            var expectedStore = await _storeService.GetStoreAsync(newStore.Guid);
            

            expectedStore.Should().BeEquivalentTo(newStore);
           
        }


        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }



    }
}
