using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using BoomaEcommerce.Core.Exceptions;
using BoomaEcommerce.Services.Authentication;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.DTO.Policies;
using BoomaEcommerce.Services.Purchases;
using BoomaEcommerce.Services.Stores;
using BoomaEcommerce.Services.Users;
using BoomaEcommerce.Tests.CoreLib;
using FluentAssertions;
using Xunit;

namespace BoomaEcommerce.AcceptanceTests
{
    public class PolicyAcceptanceTests : IAsyncLifetime
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
        private ProductDto product1_withGuid;
        private ProductDto product2_withGuid;
        private ShoppingBasketDto shopping_basket;
        private Guid _founderGuid;

        public async Task InitializeAsync()
        {
            _fixture = new Fixture();
            _founderGuid = Guid.NewGuid();
            var serviceMockFactory = new ServiceMockFactory();
            var storeService = serviceMockFactory.MockStoreService();
            var authService = serviceMockFactory.MockAuthenticationService();
            var purchaseService = serviceMockFactory.MockPurchaseService();
            var userService = serviceMockFactory.MockUserService();
            await InitUser(storeService, authService,purchaseService, userService );
            _fixture.Customize<StoreDto>(s => s
                .Without(ss => ss.Guid)
                .Without(ss => ss.Rating)
                .With(ss => ss.FounderUserGuid, _founderGuid));
            await InitPurchase(storeService);
        }

        public async Task InitPurchase(IStoresService storesService)
        {
            // create store
            _fixture.Customize<PurchaseDto>(p => p.Without(pp => pp.Guid).With(pp => pp.BuyerGuid, UserGuid));
            var store = _fixture.Create<StoreDto>();
            _store_withGuid = await _storeService.CreateStoreAsync(store);


            // create products , diary category
            _fixture.Customize<ProductDto>(p => p.Without(pp => pp.Guid)
                                           .With(pp => pp.StoreGuid, _store_withGuid.Guid)
                                           .Without(pp => pp.Rating)
                                           .With(pp => pp.Price, 2)
                                           .With(pp => pp.Category , "Diary")
                                           .With(pp => pp.Amount, 10));

            product1 = _fixture.Create<ProductDto>();
            product2 = _fixture.Create<ProductDto>();


            product1_withGuid = await storesService.CreateStoreProductAsync(product1);
            product2_withGuid = await storesService.CreateStoreProductAsync(product2);

            purchase_product2 = _fixture.Build<PurchaseProductDto>()
                                          .With(pp => pp.ProductGuid, product2_withGuid.Guid)
                                          .Without(pp => pp.Guid)
                                          .With(pp => pp.Price, 4 * product2_withGuid.Price)
                                          .With(pp => pp.Amount, 4)
                                          .Create();
            var purchase_product_lst = new List<PurchaseProductDto>();
            purchase_product_lst.Add(purchase_product2);

            _fixture.Customize<ShoppingBasketDto>(p => p.With(pp => pp.PurchaseProducts, purchase_product_lst)
                                                        .Without(pp => pp.Guid)
                                                        .With(pp => pp.StoreGuid , _store_withGuid.Guid));

            var shopping_cart = await _usersService.GetShoppingCartAsync(UserGuid);
            var shopping_basket_dto = _fixture.Create<ShoppingBasketDto>();
            shopping_basket =  await _usersService.CreateShoppingBasketAsync(shopping_cart.Guid, shopping_basket_dto);
        }

        public async Task InitUser(IStoresService storeService, IAuthenticationService authService, IPurchasesService purchasesService, IUsersService usersService)
        {
            var user = new UserDto { UserName = "Omer" };
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
        }
    
        [Fact] 
        private async Task MaxCategoryAmountPolicyDtoTest_ShouldThrowPolicyValidationException()
        {
            // Arange

            //get first store policy
            var first_store_policy = await _storeService.GetPolicyAsync(_store_withGuid.Guid);

            // create new policy 
            _fixture.Customize<CategoryAmountPolicyDto>(p => p.With(pp => pp.Type, PolicyType.MaxCategoryAmount)
                                                              .With(pp => pp.Category, "Diary")
                                                              .With(pp => pp.Amount, 3)
                                                              .Without(pp => pp.Guid));
            var policy1 = _fixture.Create<CategoryAmountPolicyDto>();

            await _storeService.CreatePurchasePolicyAsync(_store_withGuid.Guid, policy1);
            
            //create purchase product 
            purchase_product1 = _fixture.Build<PurchaseProductDto>()
                                            .With(pp => pp.ProductGuid, product1_withGuid.Guid)
                                            .Without(pp => pp.Guid)
                                            .With(pp => pp.Price, product1_withGuid.Price * 4)
                                            .With(pp => pp.Amount, 4)
                                            .Create();


            var purchaseProductList = new List<PurchaseProductDto>();
            purchaseProductList.Add(purchase_product1);

            var storePurchase = _fixture.Build<StorePurchaseDto>()
                .With(sp => sp.StoreGuid, _store_withGuid.Guid)
                .With(sp => sp.BuyerGuid, UserGuid)
                .With(sp => sp.PurchaseProducts, purchaseProductList)
                .Without(sp => sp.Guid)
                .With(sp => sp.TotalPrice, purchase_product1.Price)
                .Create();

            var store_purchase_lst = new List<StorePurchaseDto>();
            store_purchase_lst.Add(storePurchase);

            purchase = _fixture.Build<PurchaseDto>()
                .With(p => p.BuyerGuid, UserGuid)
                .With(p => p.StorePurchases, store_purchase_lst)
                .Without(p => p.Guid)
                .With(p => p.TotalPrice, storePurchase.TotalPrice)
                .Create();

            //Act 
            var result = _purchaseService.Awaiting(service => service.CreatePurchaseAsync(purchase));

            // Assert
            await result.Should().ThrowAsync<PolicyValidationException>();
        }


        [Fact]
        private async Task MaxCategoryAmountPolicyDtoTest_ShouldMakePurchase()
        {
            // Arange

            //get first store policy
            var first_store_policy = await _storeService.GetPolicyAsync(_store_withGuid.Guid);

            // create new policy 
            _fixture.Customize<CategoryAmountPolicyDto>(p => p.With(pp => pp.Type, PolicyType.MaxCategoryAmount)
                                                              .With(pp => pp.Category, "Diary")
                                                              .With(pp => pp.Amount, 3)
                                                              .Without(pp => pp.Guid));
            var policy1 = _fixture.Create<CategoryAmountPolicyDto>();

            await _storeService.CreatePurchasePolicyAsync(_store_withGuid.Guid, policy1);

            //create purchase product 
            purchase_product1 = _fixture.Build<PurchaseProductDto>()
                                            .With(pp => pp.ProductGuid, product1_withGuid.Guid)
                                            .Without(pp => pp.Guid)
                                            .With(pp => pp.Price, product1_withGuid.Price * 3)
                                            .With(pp => pp.Amount, 3)
                                            .Create();


            var purchaseProductList = new List<PurchaseProductDto>();
            purchaseProductList.Add(purchase_product1);

            var storePurchase = _fixture.Build<StorePurchaseDto>()
                .With(sp => sp.StoreGuid, _store_withGuid.Guid)
                .With(sp => sp.BuyerGuid, UserGuid)
                .With(sp => sp.PurchaseProducts, purchaseProductList)
                .Without(sp => sp.Guid)
                .With(sp => sp.TotalPrice, purchase_product1.Price)
                .Create();

            var store_purchase_lst = new List<StorePurchaseDto>();
            store_purchase_lst.Add(storePurchase);

            purchase = _fixture.Build<PurchaseDto>()
                .With(p => p.BuyerGuid, UserGuid)
                .With(p => p.StorePurchases, store_purchase_lst)
                .Without(p => p.Guid)
                .With(p => p.TotalPrice, storePurchase.TotalPrice)
                .Create();

            //Act 
            var result =  await _purchaseService.CreatePurchaseAsync(purchase);

            // Assert
            result.Should().NotBeNull();
            var product = await _storeService.GetStoreProductAsync(product1_withGuid.Guid);
            product.Amount.Should().Be(7);
        }


        [Fact]
        private async Task MinCategoryAmountPolicyDtoTest_ShouldThrowPolicyValidationException()
        {
            // Arange

           
            // create new policy 
            _fixture.Customize<CategoryAmountPolicyDto>(p => p.With(pp => pp.Type, PolicyType.MinCategoryAmount)
                                                              .With(pp => pp.Category, "Diary")
                                                              .With(pp => pp.Amount, 2)
                                                              .Without(pp => pp.Guid));
            var policy1 = _fixture.Create<CategoryAmountPolicyDto>();

            await _storeService.CreatePurchasePolicyAsync(_store_withGuid.Guid, policy1);

            //create purchase product 
            purchase_product1 = _fixture.Build<PurchaseProductDto>()
                                            .With(pp => pp.ProductGuid, product1_withGuid.Guid)
                                            .Without(pp => pp.Guid)
                                            .With(pp => pp.Price, product1_withGuid.Price * 1)
                                            .With(pp => pp.Amount, 1)
                                            .Create();


            var purchaseProductList = new List<PurchaseProductDto>();
            purchaseProductList.Add(purchase_product1);

            var storePurchase = _fixture.Build<StorePurchaseDto>()
                .With(sp => sp.StoreGuid, _store_withGuid.Guid)
                .With(sp => sp.BuyerGuid, UserGuid)
                .With(sp => sp.PurchaseProducts, purchaseProductList)
                .Without(sp => sp.Guid)
                .With(sp => sp.TotalPrice, purchase_product1.Price)
                .Create();

            var store_purchase_lst = new List<StorePurchaseDto>();
            store_purchase_lst.Add(storePurchase);

            purchase = _fixture.Build<PurchaseDto>()
                .With(p => p.BuyerGuid, UserGuid)
                .With(p => p.StorePurchases, store_purchase_lst)
                .Without(p => p.Guid)
                .With(p => p.TotalPrice, storePurchase.TotalPrice)
                .Create();

            //Act 
            var result = _purchaseService.Awaiting(service => service.CreatePurchaseAsync(purchase));

            // Assert
            await result.Should().ThrowAsync<PolicyValidationException>();
        }


        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }
    }

}
