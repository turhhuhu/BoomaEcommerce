using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using BoomaEcommerce.Core.Exceptions;
using BoomaEcommerce.Services.Authentication;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.DTO.Discounts;
using BoomaEcommerce.Services.DTO.Policies;
using BoomaEcommerce.Services.Purchases;
using BoomaEcommerce.Services.Stores;
using BoomaEcommerce.Services.Users;
using BoomaEcommerce.Tests.CoreLib;
using FluentAssertions;
using Xunit;

namespace BoomaEcommerce.AcceptanceTests
{
    public class DiscountAcceptanceTests : IAsyncLifetime
    {
        private IStoresService _storeService;
        private IPurchasesService _purchaseService;
        private IUsersService _usersService;
        private IFixture _fixture;
        private Guid _userGuid;
        private PurchaseDto _purchase;
        private StoreDto _storeWithGuid;
        private ProductDto _product1;
        private ProductDto _product2;
        private PurchaseProductDto _purchaseProduct1;
        private PurchaseProductDto _purchaseProduct2;
        private ProductDto _product1WithGuid;
        private ProductDto _product2WithGuid;

        public async Task InitializeAsync()
        {
            _fixture = new Fixture();

            var serviceMockFactory = new ServiceMockFactory();
            var storeService = serviceMockFactory.MockStoreService();
            var authService = serviceMockFactory.MockAuthenticationService();
            var purchaseService = serviceMockFactory.MockPurchaseService();
            var userService = serviceMockFactory.MockUserService();
            await InitUser(storeService, authService, purchaseService, userService);
            _fixture.Customize<StoreDto>(s => s
                .Without(ss => ss.Guid)
                .Without(ss => ss.Rating)
                .Without(ss => ss.FounderUserGuid));
            await InitPurchase(storeService);
        }

        private async Task InitPurchase(IStoresService storesService)
        {
            // create store
            _fixture.Customize<PurchaseDto>(p => p.Without(pp => pp.Guid).With(pp => pp.BuyerGuid, _userGuid));
            var store = _fixture.Create<StoreDto>();
            _storeWithGuid = await _storeService.CreateStoreAsync(store);


            // create products , diary category
            _fixture.Customize<ProductDto>(p => p.Without(pp => pp.Guid)
                                           .With(pp => pp.StoreGuid, _storeWithGuid.Guid)
                                           .Without(pp => pp.Rating)
                                           .With(pp => pp.Price, 2)
                                           .With(pp => pp.Category, "Diary")
                                           .With(pp => pp.Amount, 10));

            _product1 = _fixture.Create<ProductDto>();
            _product2 = _fixture.Create<ProductDto>();


            _product1WithGuid = await storesService.CreateStoreProductAsync(_product1);
            _product2WithGuid = await storesService.CreateStoreProductAsync(_product2);

            _purchaseProduct2 = _fixture.Build<PurchaseProductDto>()
                                          .With(pp => pp.ProductGuid, _product2WithGuid.Guid)
                                          .Without(pp => pp.Guid)
                                          .With(pp => pp.Price, 4 * _product2WithGuid.Price)
                                          .With(pp => pp.Amount, 4)
                                          .Create();
           
            var purchaseProductLst = new List<PurchaseProductDto>();
            purchaseProductLst.Add(_purchaseProduct2);

            _fixture.Customize<ShoppingBasketDto>(p => p.With(pp => pp.PurchaseProducts, purchaseProductLst)
                                                        .Without(pp => pp.Guid)
                                                        .With(pp => pp.StoreGuid, _storeWithGuid.Guid));

            var shoppingCart = await _usersService.GetShoppingCartAsync(_userGuid);
            var shoppingBasketDto = _fixture.Create<ShoppingBasketDto>();
            await _usersService.CreateShoppingBasketAsync(shoppingCart.Guid, shoppingBasketDto);
        }

        private async Task InitUser(IStoresService storeService, IAuthenticationService authService, IPurchasesService purchasesService, IUsersService usersService)
        {
            var user = new UserDto { UserName = "Omer" };
            const string password = "Omer1001";

            await authService.RegisterAsync(user, password);
            var loginResponse = await authService.LoginAsync(user.UserName, password);

            _userGuid = loginResponse.UserGuid;


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
        private async Task ApplyDiscountSimpleDiscount_ShouldUpdatePurchasePrice_WhenDiscountPolicyIsFulfilled()
        {
            // Arrange

            // create new policy 
            _fixture.Customize<CategoryAmountPolicyDto>(p => p.With(pp => pp.Type, PolicyType.MaxCategoryAmount)
                                                              .With(pp => pp.Category, "Diary")
                                                              .With(pp => pp.Amount, 3)
                                                              .Without(pp => pp.Guid));
            var policy1 = _fixture.Create<CategoryAmountPolicyDto>();

            await _storeService.CreatePurchasePolicyAsync(_storeWithGuid.Guid, policy1);

            var discount = _fixture.Build<CategoryDiscountDto>()
                .With(d => d.Category, "Diary")
                .With(d => d.Percentage, 10)
                .With(d => d.Policy, policy1)
                .With(d => d.StartTime, DateTime.MinValue)
                .With(d => d.EndTime, DateTime.MaxValue)
                .Without(d => d.Guid)
                .Create();

            await _storeService.CreateDiscountAsync(_storeWithGuid.Guid, discount);

            //create purchase product 
            _purchaseProduct1 = _fixture.Build<PurchaseProductDto>()
                                            .With(pp => pp.ProductGuid, _product1WithGuid.Guid)
                                            .Without(pp => pp.Guid)
                                            .With(pp => pp.Price, _product1WithGuid.Price * 3)
                                            .With(pp => pp.Amount, 3)
                                            .Create();


            var purchaseProductList = new List<PurchaseProductDto>();
            purchaseProductList.Add(_purchaseProduct1);

            var storePurchase = _fixture.Build<StorePurchaseDto>()
                .With(sp => sp.StoreGuid, _storeWithGuid.Guid)
                .With(sp => sp.BuyerGuid, _userGuid)
                .With(sp => sp.PurchaseProducts, purchaseProductList)
                .Without(sp => sp.Guid)
                .With(sp => sp.TotalPrice, _purchaseProduct1.Price)
                .Create();

            var storePurchaseLst = new List<StorePurchaseDto>();
            storePurchaseLst.Add(storePurchase);

            _purchase = _fixture.Build<PurchaseDto>()
                .With(p => p.BuyerGuid, _userGuid)
                .With(p => p.StorePurchases, storePurchaseLst)
                .Without(p => p.Guid)
                .With(p => p.TotalPrice, storePurchase.TotalPrice)
                .With(p => p.DiscountedPrice, (decimal)5.4)
                .Create();

            //Act 
            var purchaseDto = await _purchaseService.CreatePurchaseAsync(_purchase);

            // Assert
            purchaseDto.TotalPrice.Should().Be((decimal)6);
            purchaseDto.DiscountedPrice.Should().Be((decimal)5.4);
        }

        [Fact]
        private async Task ApplyDiscountSimpleDiscount_ShouldNOTUpdatePurchasePriceAndThrowException_WhenDiscountPolicyIsNOTFulfilled()
        {
            // Arrange
            
            // create new policy 
            _fixture.Customize<CategoryAmountPolicyDto>(p => p.With(pp => pp.Type, PolicyType.MaxCategoryAmount)
                                                              .With(pp => pp.Category, "Diary")
                                                              .With(pp => pp.Amount, 3)
                                                              .Without(pp => pp.Guid));
            var policy1 = _fixture.Create<CategoryAmountPolicyDto>();

            await _storeService.CreatePurchasePolicyAsync(_storeWithGuid.Guid, policy1);

            var discount = _fixture.Build<CategoryDiscountDto>()
                .With(d => d.Category, "Diary")
                .With(d => d.Percentage, 10)
                .With(d => d.Policy, policy1)
                .With(d => d.StartTime, DateTime.MinValue)
                .With(d => d.EndTime, DateTime.MaxValue)
                .Without(d => d.Guid)
                .Create();

            await _storeService.CreateDiscountAsync(_storeWithGuid.Guid, discount);

            //create purchase product 
            _purchaseProduct1 = _fixture.Build<PurchaseProductDto>()
                                            .With(pp => pp.ProductGuid, _product1WithGuid.Guid)
                                            .Without(pp => pp.Guid)
                                            .With(pp => pp.Price, _product1WithGuid.Price * 6)
                                            .With(pp => pp.Amount, 6)
                                            .Create();


            var purchaseProductList = new List<PurchaseProductDto> {_purchaseProduct1};

            var storePurchase = _fixture.Build<StorePurchaseDto>()
                .With(sp => sp.StoreGuid, _storeWithGuid.Guid)
                .With(sp => sp.BuyerGuid, _userGuid)
                .With(sp => sp.PurchaseProducts, purchaseProductList)
                .Without(sp => sp.Guid)
                .With(sp => sp.TotalPrice, _purchaseProduct1.Price)
                .Create();

            var storePurchaseLst = new List<StorePurchaseDto>();
            storePurchaseLst.Add(storePurchase);

            _purchase = _fixture.Build<PurchaseDto>()
                .With(p => p.BuyerGuid, _userGuid)
                .With(p => p.StorePurchases, storePurchaseLst)
                .Without(p => p.Guid)
                .With(p => p.TotalPrice, storePurchase.TotalPrice)
                .Create();

            //Act 
            var result = _purchaseService.Awaiting(service => service.CreatePurchaseAsync(_purchase));

            // Assert
            await result.Should().ThrowAsync<PolicyValidationException>();
        }


        [Fact]
        private async Task ApplyDiscountCompositeDiscount_ShouldUpdatePurchasePrice_WhenDiscountPoliciesAreFulfilled()
        {
            // Arrange

            // create new policy 
            _fixture.Customize<CategoryAmountPolicyDto>(p => p.With(pp => pp.Type, PolicyType.MaxCategoryAmount)
                                                              .With(pp => pp.Category, "Diary")
                                                              .With(pp => pp.Amount, 10)
                                                              .Without(pp => pp.Guid));
            var policy1 = _fixture.Create<CategoryAmountPolicyDto>();

            await _storeService.CreatePurchasePolicyAsync(_storeWithGuid.Guid, policy1);

            var discountDiary = _fixture.Build<CategoryDiscountDto>()
                .With(d => d.Category, "Diary")
                .With(d => d.Percentage, 10)
                .With(d => d.Policy, policy1)
                .With(d => d.StartTime, DateTime.MinValue)
                .With(d => d.EndTime, DateTime.MaxValue)
                .Without(d => d.Guid)
                .Create();

            var discountProduct = _fixture.Build<ProductDiscountDto>()
                .With(d => d.ProductGuid, _product1WithGuid.Guid)
                .With(d => d.Percentage, 10)
                .With(d => d.Policy, policy1)
                .With(d => d.StartTime, DateTime.MinValue)
                .With(d => d.EndTime, DateTime.MaxValue)
                .Without(d => d.Guid)
                .Create();

            var compositeDiscount = _fixture.Build<CompositeDiscountDto>()
                .With(d => d.Operator, OperatorTypeDiscount.Sum)
                .With(d => d.Discounts, new List<DiscountDto>())
                .With(d => d.Percentage, 10)
                .With(d => d.Policy, policy1)
                .With(d => d.StartTime, DateTime.MinValue)
                .With(d => d.EndTime, DateTime.MaxValue)
                .Without(d => d.Guid)
                .Create();

            var compDis = await _storeService.CreateDiscountAsync(_storeWithGuid.Guid, compositeDiscount);

            await _storeService.AddDiscountAsync(_storeWithGuid.Guid, compDis.Guid, discountDiary);
            
            await _storeService.AddDiscountAsync(_storeWithGuid.Guid, compDis.Guid, discountProduct);

            //create purchase product 
            _purchaseProduct1 = _fixture.Build<PurchaseProductDto>()
                                            .With(pp => pp.ProductGuid, _product1WithGuid.Guid)
                                            .Without(pp => pp.Guid)
                                            .With(pp => pp.Price, _product1WithGuid.Price * 5)
                                            .With(pp => pp.Amount, 5)
                                            .Create();


            var purchaseProductList = new List<PurchaseProductDto> {_purchaseProduct1};

            var storePurchase = _fixture.Build<StorePurchaseDto>()
                .With(sp => sp.StoreGuid, _storeWithGuid.Guid)
                .With(sp => sp.BuyerGuid, _userGuid)
                .With(sp => sp.PurchaseProducts, purchaseProductList)
                .Without(sp => sp.Guid)
                .With(sp => sp.TotalPrice, _purchaseProduct1.Price)
                .Create();

            var storePurchaseLst = new List<StorePurchaseDto>();
            storePurchaseLst.Add(storePurchase);

            _purchase = _fixture.Build<PurchaseDto>()
                .With(p => p.BuyerGuid, _userGuid)
                .With(p => p.StorePurchases, storePurchaseLst)
                .Without(p => p.Guid)
                .With(p => p.DiscountedPrice, (decimal)8.1)
                .With(p => p.TotalPrice, storePurchase.TotalPrice)
                .Create();

            //Act 
            var purchaseDto = await _purchaseService.CreatePurchaseAsync(_purchase);

            // Assert
            purchaseDto.TotalPrice.Should().Be(10);
            purchaseDto.DiscountedPrice.Should().Be((decimal)8.1);
        }


        [Fact]
        private async Task ApplyDiscountCompositeDiscount_ShouldApplyOnlyMatchingDiscounts_WhenDiscountPoliciesAreNotDisjointed()
        {
            // Arrange
            // create new policy 
            _fixture.Customize<CategoryAmountPolicyDto>(p => p.With(pp => pp.Type, PolicyType.MaxCategoryAmount)
                                                              .With(pp => pp.Category, "Diary")
                                                              .With(pp => pp.Amount, 10)
                                                              .Without(pp => pp.Guid));
            var policy1 = _fixture.Create<CategoryAmountPolicyDto>();

            await _storeService.CreatePurchasePolicyAsync(_storeWithGuid.Guid, policy1);

            var discountDiary = _fixture.Build<CategoryDiscountDto>()
                .With(d => d.Category, "Diary")
                .With(d => d.Percentage, 20)
                .With(d => d.Policy, policy1)
                .With(d => d.StartTime, DateTime.MinValue)
                .With(d => d.EndTime, DateTime.MaxValue)
                .Without(d => d.Guid)
                .Create();

            var discountProduct = _fixture.Build<ProductDiscountDto>()
                .With(d => d.ProductGuid, _product1WithGuid.Guid)
                .With(d => d.Percentage, 10)
                .With(d => d.Policy, policy1)
                .With(d => d.StartTime, DateTime.MinValue)
                .With(d => d.EndTime, DateTime.MaxValue)
                .Without(d => d.Guid)
                .Create();

            var compositeDiscount = _fixture.Build<CompositeDiscountDto>()
                .With(d => d.Operator, OperatorTypeDiscount.Sum)
                .With(d => d.Discounts, new List<DiscountDto>())
                .Without(d => d.Percentage)
                .With(d => d.Policy, policy1)
                .With(d => d.StartTime, DateTime.MinValue)
                .With(d => d.EndTime, DateTime.MaxValue)
                .Without(d => d.Guid)
                .Create();

            var compDis = await _storeService.CreateDiscountAsync(_storeWithGuid.Guid, compositeDiscount);

            await _storeService.AddDiscountAsync(_storeWithGuid.Guid, compDis.Guid, discountDiary);

            await _storeService.AddDiscountAsync(_storeWithGuid.Guid, compDis.Guid, discountProduct);
            


            //create purchase product 
            _purchaseProduct1 = _fixture.Build<PurchaseProductDto>()
                                            .With(pp => pp.ProductGuid, _product1WithGuid.Guid)
                                            .Without(pp => pp.Guid)
                                            .With(pp => pp.Price, _product1WithGuid.Price * 5)
                                            .With(pp => pp.Amount, 5)
                                            .Create();

            _purchaseProduct2 = _fixture.Build<PurchaseProductDto>()
                .With(pp => pp.ProductGuid, _product2WithGuid.Guid)
                .Without(pp => pp.Guid)
                .With(pp => pp.Price, _product2WithGuid.Price * 5)
                .With(pp => pp.Amount, 5)
                .Create();


            var purchaseProductList = new List<PurchaseProductDto>();
            purchaseProductList.Add(_purchaseProduct1);
            purchaseProductList.Add(_purchaseProduct2);

            var storePurchase = _fixture.Build<StorePurchaseDto>()
                .With(sp => sp.StoreGuid, _storeWithGuid.Guid)
                .With(sp => sp.BuyerGuid, _userGuid)
                .With(sp => sp.PurchaseProducts, purchaseProductList)
                .Without(sp => sp.Guid)
                .With(sp => sp.TotalPrice, _purchaseProduct1.Price + _purchaseProduct2.Price)
                .Create();

            var storePurchaseLst = new List<StorePurchaseDto> {storePurchase};

            _purchase = _fixture.Build<PurchaseDto>()
                .With(p => p.BuyerGuid, _userGuid)
                .With(p => p.StorePurchases, storePurchaseLst)
                .Without(p => p.Guid)
                .With(p => p.TotalPrice, storePurchase.TotalPrice)
                .With(p => p.DiscountedPrice, (decimal)15.2)
                .Create();

            //Act 
            var purchaseDto = await _purchaseService.CreatePurchaseAsync(_purchase);

            // Assert
            purchaseDto.TotalPrice.Should().Be(20);
            purchaseDto.DiscountedPrice.Should().Be((decimal)15.2);
        }


        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
