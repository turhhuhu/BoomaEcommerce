using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using BoomaEcommerce.Core.Exceptions;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services;
using BoomaEcommerce.Services.Authentication;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Purchases;
using BoomaEcommerce.Services.Stores;
using BoomaEcommerce.Services.Tests;
using BoomaEcommerce.Tests.CoreLib;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Xunit;

namespace BoomaEcommerce.AcceptanceTests
{
    public class NotificationsAcceptanceTests : IAsyncLifetime
    {

        private IPurchasesService _purchasesServiceOwner;
        private IPurchasesService _purchasesServiceBuyer;
        private PurchaseDto _purchase;
        private PurchaseDto _badPurchase;
        private IFixture _fixture;
        private StoreOwnershipDto _storeOwnership;
        private NotificationPublisherStub _notificationPublisher;
        private IStoresService _ownerStoreService;
        private IStoresService _notOwnerStoreService;
        private List<NotificationDto> _ownerNotifications = new List<NotificationDto>();
        private List<NotificationDto> _notOwnerNotifications = new List<NotificationDto>();

        public async Task InitializeAsync()
        {
            _fixture = new Fixture();
            _fixture.Customize<StoreDto>(s =>
                s.Without(ss => ss.Guid).Without(ss => ss.Rating));

            var serviceMockFactory = new ServiceMockFactory();
            var storeService = serviceMockFactory.MockStoreService();
            var authService = serviceMockFactory.MockAuthenticationService();
            var mutualPurchaseService = serviceMockFactory.MockPurchaseService();
            _notificationPublisher = serviceMockFactory.GetNotificationPublisherStub();
            await InitOwnerUser(storeService, authService, mutualPurchaseService);
            var product = await CreateStoreProduct(storeService);
            await PurchaseProduct(mutualPurchaseService, product, authService, storeService);

            //_fixture.Customize<ProductDto>(p => p.Without(pp => pp.Id).Without(pp => pp.Rating)
                //.With(pp => pp.StoreGuid, _storeOwnership.Store.Id));
        }

        private async Task InitOwnerUser(IStoresService storeService, IAuthenticationService authService, 
            IPurchasesService purchasesService)
        {
            var user = new UserDto { UserName = "Owner" };
            const string password = "OwnerUserSuperSecretPassword";

            await authService.RegisterAsync(user, password);
            var loginResponse = await authService.LoginAsync(user.UserName, password);

            var fixtureStore = _fixture
                .Build<StoreDto>()
                .With(s => s.FounderUserGuid, loginResponse.UserGuid)
                .Without(s => s.Rating)
                .Without(s => s.Guid)
                .Create();

            await storeService.CreateStoreAsync(fixtureStore);


            _storeOwnership = (await storeService.GetAllStoreOwnerShipsAsync(loginResponse.UserGuid)).First();
            var result = SecuredPurchaseService.CreateSecuredPurchaseService(loginResponse.Token,
                ServiceMockFactory.Secret, purchasesService, out _purchasesServiceOwner);
            if (!result)
            {
                throw new Exception("This shouldn't happen");
            }

            var result2 = SecuredStoreService.CreateSecuredStoreService(loginResponse.Token,
                ServiceMockFactory.Secret, storeService, out _ownerStoreService);
            if (!result2)
            {
                throw new Exception("This shouldn't happen");
            }


            await _notificationPublisher.addNotifiedUser(loginResponse.UserGuid, _ownerNotifications);
            //_purchasesServiceOwner.SetNotificationPublisher(_notificationPublisher);

        }

        private async Task PurchaseProduct(IPurchasesService purchasesService, ProductDto productDto,
            IAuthenticationService authenticationService, IStoresService storesService)
        {

            var notOwnerUser = new UserDto { UserName = "Benny" };
            const string notOwnerPassword = "SuperSecretPassword";
            await authenticationService.RegisterAsync(notOwnerUser, notOwnerPassword);
            var notOwnerLoginResponse = await authenticationService.LoginAsync(notOwnerUser.UserName, notOwnerPassword);
            var result = SecuredPurchaseService.CreateSecuredPurchaseService(notOwnerLoginResponse.Token,
                ServiceMockFactory.Secret, purchasesService, out _purchasesServiceBuyer);
            if (!result)
            {
                throw new Exception("This shouldn't happen");
            }

            var result2 = SecuredStoreService.CreateSecuredStoreService(notOwnerLoginResponse.Token,
                ServiceMockFactory.Secret, storesService, out _notOwnerStoreService);

            if (!result2)
            {
                throw new Exception("This shouldn't happen");
            }

            await _notificationPublisher.addNotifiedUser(notOwnerLoginResponse.UserGuid, _notOwnerNotifications);
            //_purchasesServiceOwner.SetNotificationPublisher(_notificationPublisher);

            var purchaseDto = new PurchaseDto
            {
                BuyerGuid = notOwnerLoginResponse.UserGuid,
                TotalPrice = 10,
                DiscountedPrice = 10,
                StorePurchases = new List<StorePurchaseDto>
                {
                    new()
                    {
                        BuyerGuid = notOwnerLoginResponse.UserGuid,
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


            var badPurchaseDto = new PurchaseDto
            {
                BuyerGuid = notOwnerLoginResponse.UserGuid,
                TotalPrice = 10,
                DiscountedPrice = 10,
                StorePurchases = new List<StorePurchaseDto>
                {
                    new()
                    {
                        BuyerGuid = notOwnerLoginResponse.UserGuid,
                        StoreGuid = productDto.StoreGuid,
                        TotalPrice = 10,
                        PurchaseProducts = new List<PurchaseProductDto>
                        {
                            new()
                            {
                                Amount = 1,
                                Price = 10,
                                ProductGuid = Guid.NewGuid()
                            }
                        }
                    }
                }
            };

            _badPurchase = badPurchaseDto;
        }

        private async Task<ProductDto> CreateStoreProduct(IStoresService storeService)
        {
            var fixtureProductDto = _fixture
                .Build<ProductDto>()
                .With(s => s.StoreGuid, _storeOwnership.Store.Guid)
                .With(p => p.Amount, 10)
                .With(p => p.Price, 10)
                .With(p => p.Guid, Guid.NewGuid())
                .Without(p => p.Rating)
                .Create();

            var productDto = await storeService.CreateStoreProductAsync(fixtureProductDto);
            if (productDto is null)
            {
                throw new Exception("This shouldn't happen");
            }
            return productDto;
        }






        [Fact]
        public async Task
            CreatePurchaseProduct_ShouldNotifyOwner_WhenPurchaseWasCreatedSuccessfully()
        {
            var purchaseProductDetails = _fixture.Build<PurchaseDetailsDto>()
                .With(pd => pd.Purchase, _purchase)
                .Create();
            // Act 
            var resTask = await _purchasesServiceBuyer.CreatePurchaseAsync(purchaseProductDetails);

            // Assert
            _ownerNotifications.Should().NotBeEmpty();
        }

        [Fact]
        public async Task
            CreatePurchaseProduct_ShouldNOTNotifyOwner_WhenPurchaseWasCreatedUnsuccessfully()
        {
            var purchaseProductDetails = _fixture.Build<PurchaseDetailsDto>()
                .With(pd => pd.Purchase, _badPurchase)
                .Create();
            // Act 
            var resTask = await _purchasesServiceBuyer.CreatePurchaseAsync(purchaseProductDetails);

            // Assert
            _ownerNotifications.Should().BeEmpty();
        }

        [Fact]
        public async Task
            CreatePurchaseProduct_ShouldNOTNotifyBuyer_WhenPurchaseWasCreatedSuccessfully()
        {
            var purchaseProductDetails = _fixture.Build<PurchaseDetailsDto>()
                .With(pd => pd.Purchase, _purchase)
                .Create();
            // Act 
            var resTask = await _purchasesServiceBuyer.CreatePurchaseAsync(purchaseProductDetails);

            // Assert
            _notOwnerNotifications.Count.Should().Be(0);
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
