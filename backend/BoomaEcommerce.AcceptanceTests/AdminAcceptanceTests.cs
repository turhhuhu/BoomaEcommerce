using System;
using System.Collections.Generic;
using System.Linq;
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
    public class AdminAcceptanceTests : IAsyncLifetime
    {
        private string _adminUsername = "Ori";
        private string _adminPassword = "Ori1234";
        private IStoresService _adminStoreService;
        private IPurchasesService _adminPurchaseService;
        private PurchaseDto _purchase;

        private StoreDto _storeDto;

        private IFixture _fixture;
        public async Task InitializeAsync()
        {
            _fixture = new Fixture();
            _fixture.Customize<StoreDto>(s => s.Without(ss => ss.Guid));
            
            var serviceMockFactory = new ServiceMockFactory(); 
            var storeService = serviceMockFactory.MockStoreService();
            var purchaseService = serviceMockFactory.MockPurchaseService();
            var authService = serviceMockFactory.MockAuthenticationService();

            await InitStoreWithProductAndPurchase(storeService, authService, purchaseService);
            
            await InitAdminServices(storeService, purchaseService, authService);
        }

        private async Task InitAdminServices(IStoresService storeService, IPurchasesService purchaseService, IAuthenticationService authService)
        {
            var registrationRes = await authService.RegisterAdminAsync(_adminUsername, _adminPassword);
            var loginResponse = await authService.LoginAsync(_adminUsername, _adminPassword);

            var storeServiceRes = SecuredStoreService.CreateSecuredStoreService(loginResponse.Token,
                ServiceMockFactory.Secret, storeService, out _adminStoreService);
            if (!storeServiceRes)
            {
                throw new Exception("This shouldn't happen");
            }

            var purchaseServiceRes = SecuredPurchaseService.CreateSecuredPurchaseService(loginResponse.Token,
                ServiceMockFactory.Secret, purchaseService, out _adminPurchaseService);
            if (!purchaseServiceRes)
            {
                throw new Exception("This shouldn't happen");
            }
        }

        private async Task InitStoreWithProductAndPurchase(IStoresService storeService,
            IAuthenticationService authService, IPurchasesService purchasesService)
        {
            const string username = "Arik";
            const string password = "Arik1337";

            await authService.RegisterAsync(username, password);
            var loginResponse = await authService.LoginAsync(username, password);

            await CreateStore(storeService, loginResponse);

            var productDto = await CreateStoreProduct(storeService);

            await PurchaseProduct(purchasesService, productDto, authService);
        }

        private async Task CreateStore(IStoresService storeService, AuthenticationResult loginResponse)
        {
            var fixtureStoreDto = _fixture
                .Build<StoreDto>()
                .With(s => s.StoreFounder, loginResponse.User)
                .Without(s => s.Guid)
                .Without(s => s.Rating)
                .Create();

            await storeService.CreateStoreAsync(fixtureStoreDto);

            _storeDto = (await storeService.GetStoresAsync()).First();
        }

        private async Task<ProductDto> CreateStoreProduct(IStoresService storeService)
        {
            var fixtureProductDto = _fixture
                .Build<ProductDto>()
                .With(s => s.Store, _storeDto)
                .With(p => p.Amount, 10)
                .With(p => p.Price, 10)
                .Without(p => p.Rating)
                .Without(p => p.Guid)
                .Create();

            var productDto = await storeService.CreateStoreProductAsync(fixtureProductDto);
            if (productDto is null)
            {
                throw new Exception("This shouldn't happen");
            }
            return productDto;
        }

        private async Task PurchaseProduct(IPurchasesService purchasesService, ProductDto productDto,
            IAuthenticationService authenticationService)
        {
            var buyerUserName = "Matan";
            var buyerPassword = "Matan1234";
            await authenticationService.RegisterAsync(buyerUserName, buyerPassword);
            var buyerToken = await authenticationService.LoginAsync(buyerUserName, buyerPassword);
            
            var purchaseDto = new PurchaseDto
            {
                Buyer = buyerToken.User,
                TotalPrice = 10,
                StorePurchases = new List<StorePurchaseDto>
                {
                    new()
                    {
                        Buyer = buyerToken.User,
                        Store = productDto.Store,
                        TotalPrice = 10,
                        PurchaseProducts = new List<PurchaseProductDto>
                        {
                            new()
                            {
                                Amount = 1,
                                Price = 10,
                                Product = productDto
                            }
                        }
                    }
                }
            };
            _purchase = purchaseDto;
            var didPurchasedSucceeded = await purchasesService.CreatePurchaseAsync(purchaseDto);
            if (!didPurchasedSucceeded)
            {
                throw new Exception("This shouldn't happen");
            }
        }

        [Fact]
        public async Task GetStorePurchaseHistory_ReturnStorePurchaseHistory_WhenStoreExists()
        {
            // Arrange
            var storeGuid = _storeDto.Guid;

            // Act
            var result = await _adminStoreService.GetStorePurchaseHistory(storeGuid);
            var storePurchase = result.First();
            var purchaseProduct = storePurchase.PurchaseProducts.First();
            var realStorePurchase = _purchase.StorePurchases.First();
            var realPurchaseProduct = realStorePurchase.PurchaseProducts.First();

            // Assert
            storePurchase.Should().BeEquivalentTo(realStorePurchase, opt => opt.Excluding(p => p.Guid).Excluding(p => p.PurchaseProducts));
            purchaseProduct.Should().BeEquivalentTo(realPurchaseProduct, opt => opt.Excluding(p => p.Guid).Excluding(p => p.Product.Amount));
        }
        

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }
    }
}