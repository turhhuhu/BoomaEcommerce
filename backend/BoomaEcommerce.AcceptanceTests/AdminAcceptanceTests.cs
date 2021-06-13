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
        private AdminUserDto _adminUser = new() { UserName = "Ori" };
        private string _adminPassword = "Ori1234";

        private IStoresService _adminStoreService;
        private IPurchasesService _adminPurchaseService;
        private PurchaseDto _purchase;
        private StoreDto _storeDto;
        private NotificationPublisherStub _notificationPublisher;


        private IFixture _fixture;
        public async Task InitializeAsync()
        {
            _fixture = new Fixture();
            _fixture.Customize<StoreDto>(s => s.Without(ss => ss.Guid));
            
            var serviceMockFactory = new ServiceMockFactory(); 
            var storeService = serviceMockFactory.MockStoreService();
            var purchaseService = serviceMockFactory.MockPurchaseService();
            _notificationPublisher = serviceMockFactory.GetNotificationPublisherStub();
            var authService = serviceMockFactory.MockAuthenticationService();

            await InitStoreWithProductAndPurchase(storeService, authService, purchaseService);
            
            await InitAdminServices(storeService, purchaseService, authService);
        }

        private async Task InitAdminServices(IStoresService storeService, IPurchasesService purchaseService, IAuthenticationService authService)
        {
            await authService.RegisterAdminAsync(_adminUser, _adminPassword);
            var loginResponse = await authService.LoginAsync(_adminUser.UserName, _adminPassword);

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

            await _notificationPublisher.addNotifiedUser(loginResponse.UserGuid, new List<NotificationDto>());
        }

        private async Task InitStoreWithProductAndPurchase(IStoresService storeService,
            IAuthenticationService authService, IPurchasesService purchasesService)
        {
            var user = new UserDto {UserName = "Arik"};
            const string password = "Arik1337";

            await authService.RegisterAsync(user, password);
            var loginResponse = await authService.LoginAsync(user.UserName, password);

            await _notificationPublisher.addNotifiedUser(loginResponse.UserGuid, new List<NotificationDto>());

            await CreateStore(storeService, loginResponse);

            var productDto = await CreateStoreProduct(storeService);

            await PurchaseProduct(purchasesService, productDto, authService);
        }

        private async Task CreateStore(IStoresService storeService, AuthenticationResult loginResponse)
        {
            var fixtureStoreDto = _fixture
                .Build<StoreDto>()
                .With(s => s.FounderUserGuid, loginResponse.UserGuid)
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
                .With(s => s.StoreGuid, _storeDto.Guid)
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
            var buyerUser = new UserDto { UserName = "Matan" };
            const string buyerPassword = "Matan1234";
            await authenticationService.RegisterAsync(buyerUser, buyerPassword);
            var buyerToken = await authenticationService.LoginAsync(buyerUser.UserName, buyerPassword);
            
            var purchaseDto = new PurchaseDto
            {
                UserBuyerGuid = buyerToken.UserGuid,
                TotalPrice = 10,
                DiscountedPrice = 10,
                StorePurchases = new List<StorePurchaseDto>
                {
                    new()
                    {
                        BuyerGuid = buyerToken.UserGuid,
                        StoreGuid = productDto.StoreGuid,
                        TotalPrice = 10,
                        PurchaseProducts = new List<PurchaseProductDto>
                        {
                            new()
                            {
                                Amount = 1,
                                Price = 10,
                                ProductGuid = productDto.Guid
                            }
                        }
                    }
                }
            };
            _purchase = purchaseDto;
            var purchaseProductDetails = _fixture.Build<PurchaseDetailsDto>()
                .With(pd => pd.Purchase, _purchase)
                .Create();
            var didPurchasedSucceeded = await purchasesService.CreatePurchaseAsync(purchaseProductDetails);
            if (didPurchasedSucceeded == null)
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
            var result = await _adminStoreService.GetStorePurchaseHistoryAsync(storeGuid);
            var storePurchase = result.First();
            var purchaseProduct = storePurchase.PurchaseProducts.First();
            var realStorePurchase = _purchase.StorePurchases.First();
            var realPurchaseProduct = realStorePurchase.PurchaseProducts.First();

            // Assert
            storePurchase.Should().BeEquivalentTo(realStorePurchase, 
                opt => opt
                        .Excluding(p => p.Guid)
                        .Excluding(p => p.PurchaseProducts));
            purchaseProduct.Should().BeEquivalentTo(realPurchaseProduct, 
                opt => opt
                    .Excluding(p => p.Guid)
                    .Excluding(p => p.ProductGuid));
        }
        
        [Fact]
        public async Task GetStorePurchaseHistory_ReturnEmptyCollection_WhenStoreDoesNotExists()
        {
            // Arrange
            var nonExistingStoreGuid = Guid.NewGuid();

            // Act
            var result = 
                await _adminStoreService.GetStorePurchaseHistoryAsync(nonExistingStoreGuid);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllUserPurchaseHistoryAsync_ReturnsAllUserPurchaseHistory_WhenUserExists()
        {
            // Arrange
            var userGuid = _purchase.UserBuyerGuid;
            
            // Act
            var result = await _adminPurchaseService.GetAllUserPurchaseHistoryAsync(userGuid!.Value);
            
            // Assert
            result.Should().NotBeNull().And.NotBeEmpty();
            var purchase = result.First();
            purchase.Should().BeEquivalentTo(_purchase,
                opt => opt
                    .Excluding(p => p.StorePurchases)
                    .Excluding(p => p.Guid)
                    .Excluding(p => p.Buyer));
            var storePurchase = purchase.StorePurchases.First();
            var purchaseProduct = storePurchase.PurchaseProducts.First();
            var realStorePurchase = _purchase.StorePurchases.First();
            var realPurchaseProduct = realStorePurchase.PurchaseProducts.First();
            storePurchase.Should().BeEquivalentTo(realStorePurchase, 
                opt => opt
                    .Excluding(p => p.Guid)
                    .Excluding(p => p.PurchaseProducts));
            purchaseProduct.Should().BeEquivalentTo(realPurchaseProduct, 
                opt => opt
                    .Excluding(p => p.Guid)
                    .Excluding(p => p.ProductGuid));
        }
        
        [Fact]
        public async Task GetAllUserPurchaseHistoryAsync_ReturnEmptyCollection_WhenUserDoesNotExists()
        {
            // Arrange
            var nonExistingUserGuid = Guid.NewGuid();
            
            // Act
            var result = 
                await _adminPurchaseService.GetAllUserPurchaseHistoryAsync(nonExistingUserGuid);

            // Assert
            result.Should().BeEmpty();
        }
        

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }
    }
}