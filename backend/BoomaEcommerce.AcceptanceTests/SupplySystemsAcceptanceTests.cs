using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.Authentication;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.External;
using BoomaEcommerce.Services.Purchases;
using BoomaEcommerce.Services.Stores;
using BoomaEcommerce.Services.Users;
using BoomaEcommerce.Tests.CoreLib;
using FluentAssertions;
using Moq;
using Xunit;

namespace BoomaEcommerce.AcceptanceTests
{
    public class SupplySystemsAcceptanceTests : IAsyncLifetime
    {
        private IPurchasesService _purchaseService;
        private IFixture _fixture;

        private IStoresService _storeService;
        private Guid UserGuid;
        private PurchaseDto purchase;
        private StoreDto _store_withGuid;
        private List<PurchaseProductDto> purchase_product_lst;
        private ProductDto product1;
        private ProductDto product2;
        private PurchaseProductDto purchase_product1;
        private PurchaseProductDto purchase_product2;
        private Guid _founderGuid;

        public async Task InitializeAsync()
        {
            _fixture = new Fixture();
            _founderGuid = Guid.NewGuid();
            var serviceMockFactory = new ServiceMockFactory();
            var storeService = serviceMockFactory.MockStoreService();
            var authService = serviceMockFactory.MockAuthenticationService();
            var purchasesServicePaymentFail = serviceMockFactory.MockPurchaseServiceWithCollapsingSupplyExternalSystem();
            await InitUser(storeService, authService, purchasesServicePaymentFail);
            _fixture.Customize<StoreDto>(s => s
                .Without(ss => ss.Guid)
                .Without(ss => ss.Rating)
                .With(ss => ss.FounderUserGuid, _founderGuid));
            await InitPurchase(storeService);
        }


        private async Task InitPurchase(IStoresService storesService)
        {

            var store = _fixture.Create<StoreDto>();
            _fixture.Customize<PurchaseDto>(p => p.Without(pp => pp.Guid).With(pp => pp.BuyerGuid, UserGuid));

            _store_withGuid = await _storeService.CreateStoreAsync(store);
            _fixture.Customize<ProductDto>(p => p.Without(pp => pp.Guid).With(pp => pp.StoreGuid, _store_withGuid.Guid).Without(pp => pp.Rating).With(pp => pp.Price, 2).With(pp => pp.Amount, 10));

            product1 = _fixture.Create<ProductDto>();
            product2 = _fixture.Create<ProductDto>();


            var product1_withGuid = await storesService.CreateStoreProductAsync(product1);
            var product2_withGuid = await storesService.CreateStoreProductAsync(product2);


            purchase_product1 = _fixture.Build<PurchaseProductDto>()
                                            .With(pp => pp.ProductGuid, product1_withGuid.Guid)
                                            .Without(pp => pp.Guid)
                                            .With(pp => pp.Price, product1_withGuid.Price)
                                            .With(pp => pp.Amount, 2)
                                            .Create();

            purchase_product2 = _fixture.Build<PurchaseProductDto>()
                                            .With(pp => pp.ProductGuid, product2_withGuid.Guid)
                                            .With(pp => pp.Price, product2_withGuid.Price)
                                            .With(pp => pp.Amount, 7)
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

        private async Task InitUser(IStoresService storeService, IAuthenticationService authService, IPurchasesService purchasesService)
        {
            var user = new UserDto { UserName = "Benjaminio" };
            const string password = "OMG";

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
        }

        [Fact]
        public async Task PurchaseProduct_ShouldNOTPerformPurchase_WhenPaymentSystemFails()
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
                .With(p => p.BuyerGuid, UserGuid)
                .Without(p => p.Guid)
                .Create();
            
            var purchaseProductDetails = _fixture.Build<PurchaseDetailsDto>()
                .With(pd => pd.Purchase, myPurchase)
                .Create();

            List<int?> inventoryList = new List<int?>();
            inventoryList.Add(product1.Amount);
            inventoryList.Add(product2.Amount);

            inventoryList.Sort();

            // Act 

            var purchaseWasSuccessful = await _purchaseService.CreatePurchaseAsync(purchaseProductDetails);

            // Assert
            purchaseWasSuccessful.Should().BeNull(); // payment client should throw exception
            // check product wasn't added
            List<int?> inventoryListAfterFailedPurchase = new List<int?>();
            inventoryListAfterFailedPurchase.Add(product1.Amount);
            inventoryListAfterFailedPurchase.Add(product2.Amount);

            inventoryListAfterFailedPurchase.Sort();
            

            inventoryListAfterFailedPurchase.Should().BeEquivalentTo(inventoryList);
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

    }
}
