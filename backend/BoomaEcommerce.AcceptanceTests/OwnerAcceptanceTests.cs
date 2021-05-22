using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using BoomaEcommerce.Core.Exceptions;
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
    public class OwnerAcceptanceTests : IAsyncLifetime
    {
        private IStoresService _ownerStoreService;
        private StoreOwnershipDto _storeOwnership;

        private Guid _notOwnerUser;
        private IStoresService _notOwnerStoreService;
        private NotificationPublisherStub _notificationPublisher;

        private PurchaseDto _purchase;
        private IFixture _fixture;

        public async Task InitializeAsync()
        {

            _fixture = new Fixture();
            _fixture.Customize<StoreDto>(s =>
                s.Without(ss => ss.Guid).Without(ss => ss.Rating));

            var serviceMockFactory = new ServiceMockFactory();
            var storeService = serviceMockFactory.MockStoreService();
            var authService = serviceMockFactory.MockAuthenticationService();
            var purchaseService = serviceMockFactory.MockPurchaseService();
            _notificationPublisher = serviceMockFactory.GetNotificationPublisherStub();
            await InitOwnerUser(storeService, authService);
            await InitNotOwnerUser(storeService, authService);
            var product = await CreateStoreProduct(storeService);
            await PurchaseProduct(purchaseService, product, authService);
            
            _fixture.Customize<ProductDto>(p => p.Without(pp => pp.Guid).Without(pp => pp.Rating)
                .With(pp => pp.StoreGuid, _storeOwnership.Store.Guid));
        }

        private async Task InitOwnerUser(IStoresService storeService, IAuthenticationService authService)
        {
            var user = new UserDto {UserName = "Arik"};
            const string password = "Arik1337";

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
            var result = SecuredStoreService.CreateSecuredStoreService(loginResponse.Token,
                ServiceMockFactory.Secret, storeService, out _ownerStoreService);
            if (!result)
            {
                throw new Exception("This shouldn't happen");

            }

            await _notificationPublisher.addNotifiedUser(loginResponse.UserGuid);
        }

        private async Task InitNotOwnerUser(IStoresService storeService, IAuthenticationService authService)
        {
            var notOwnerUser = new UserDto {UserName = "Ori"};
            const string notOwnerPassword = "Ori1234";
            await authService.RegisterAsync(notOwnerUser, notOwnerPassword);
            var notOwnerLoginResponse = await authService.LoginAsync(notOwnerUser.UserName, notOwnerPassword);
            _notOwnerUser =notOwnerLoginResponse.UserGuid; 
            var result = SecuredStoreService.CreateSecuredStoreService(notOwnerLoginResponse.Token,
                ServiceMockFactory.Secret, storeService, out _notOwnerStoreService);

            if (!result)
            {
                throw new Exception("This shouldn't happen");
            }

            await _notificationPublisher.addNotifiedUser(notOwnerLoginResponse.UserGuid);


        }
        
        private async Task PurchaseProduct(IPurchasesService purchasesService, ProductDto productDto,
            IAuthenticationService authenticationService)
        {
            var buyerUser = new UserDto {UserName = "Matan"};
            const string buyerPassword = "Matan1234";
            await authenticationService.RegisterAsync(buyerUser, buyerPassword);
            var buyerToken = await authenticationService.LoginAsync(buyerUser.UserName, buyerPassword);
            
            var purchaseDto = new PurchaseDto
            {
                BuyerGuid = buyerToken.UserGuid,
                TotalPrice = 10,
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
            // added 
            
            var didPurchasedSucceeded = await purchasesService.CreatePurchaseAsync(purchaseDto);
            if (!didPurchasedSucceeded)
            {
                throw new Exception("This shouldn't happen");
            }
        }
        
        private async Task<ProductDto> CreateStoreProduct(IStoresService storeService)
        {
            var fixtureProductDto = _fixture
                .Build<ProductDto>()
                .With(s => s.StoreGuid, _storeOwnership.Store.Guid)
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

        #region ProductCRUD

        [Fact]
        public async Task CreateStoreProductAsync_ReturnsTrueAndCreatesProduct_WhenUserIsStoreOwner()
        {
            // Arrange
            var productDto = _fixture.Create<ProductDto>();

            // Act
            var result = await _ownerStoreService.CreateStoreProductAsync(productDto);

            // Assert
            result.Should().BeEquivalentTo(productDto, opt => opt.Excluding(p => p.Guid).Excluding(p => p.Rating));
            var product = await _ownerStoreService.GetStoreProductAsync(result.Guid);
            product.Should().BeEquivalentTo(result);
        }



        [Fact]
        public async Task CreateStoreProductAsync_ThrowsUnAuthorizedException_WhenUserIsNotStoreOwner()
        {
            // Arrange
            var productDto = _fixture.Create<ProductDto>();

            // Act
            var act = _notOwnerStoreService.Awaiting(storeService => storeService.CreateStoreProductAsync(productDto));

            // Assert
            await act.Should().ThrowAsync<UnAuthorizedException>();

        }

        [Fact]
        public async Task DeleteStoreProductAsync_ThrowsUnAuthorizedException_WhenUserIsNotStoreOwner()
        {
            // Arrange
            var productDto = _fixture.Create<ProductDto>();

            var resultProduct = await _ownerStoreService.CreateStoreProductAsync(productDto);

            // Act
            var act = _notOwnerStoreService.Awaiting(x => x.DeleteProductAsync(resultProduct.Guid));
            // Assert
            var notActuallyRemovedProduct = await _notOwnerStoreService.GetStoreProductAsync(resultProduct.Guid);
            await act.Should().ThrowAsync<UnAuthorizedException>();
            notActuallyRemovedProduct.Should().BeEquivalentTo(resultProduct);
        }

        [Fact]
        public async Task DeleteStoreProductAsync_DeletesProductSuccessfully_WhenUserIsStoreOwner()
        {
            // Arrange
            var productDto = _fixture.Create<ProductDto>();

            var resultProduct = await _ownerStoreService.CreateStoreProductAsync(productDto);
            // Act
            var result = await _ownerStoreService.DeleteProductAsync(resultProduct.Guid);
            // Assert
            var removedProduct = await _ownerStoreService.GetStoreProductAsync(resultProduct.Guid);
            result.Should().BeTrue();
            removedProduct.Should().BeNull();
        }

        [Fact]
        public async Task UpdateStoreProductAsync_UpdatesProductSuccessfully_WhenProductIsValidAndUserIsOwner()
        {
            // Arrange
            var productDto = _fixture.Create<ProductDto>();
            var resultProduct = await _ownerStoreService.CreateStoreProductAsync(productDto);
            var updateProduct = _fixture.Create<ProductDto>();
            updateProduct.Guid = resultProduct.Guid;

            // Act
            var result = await _ownerStoreService.UpdateProductAsync(updateProduct);
            var updatedProduct = await _ownerStoreService.GetStoreProductAsync(updateProduct.Guid);

            // Assert
            result.Should().BeTrue();
            updatedProduct.Should()
                .BeEquivalentTo(updateProduct, options => options.Excluding(product => product.Rating));
        }

        [Fact]
        public async Task UpdateStoreProductAsync_ThrowsValidationException_WhenProductIsNotValidAndUserIsOwner()
        {
            // Arrange
            var productDto = _fixture.Create<ProductDto>();
            var resultProduct = await _ownerStoreService.CreateStoreProductAsync(productDto);
            var updateProduct = new ProductDto {Guid = resultProduct.Guid, StoreGuid = productDto.StoreGuid, Amount = -1};

            // Act
            var result = _ownerStoreService.Awaiting(storeService => storeService.UpdateProductAsync(updateProduct));

            // Assert
            await result.Should().ThrowAsync<ValidationException>();
            var updatedProduct = await _ownerStoreService.GetStoreProductAsync(updateProduct.Guid);
            updatedProduct.Should()
                .NotBeEquivalentTo(updateProduct, options => options.Excluding(product => product.Rating));
        }

        [Fact]
        public async Task UpdateStoreProductAsync_ThrowsAuthorizationException_WhenProductIsValidAndUserIsNotOwner()
        {
            // Arrange
            var productDto = _fixture.Create<ProductDto>();
            var resultProduct = await _ownerStoreService.CreateStoreProductAsync(productDto);

            var updateProduct = _fixture.Create<ProductDto>();
            updateProduct.Guid = resultProduct.Guid;

            // Act
            var result = _notOwnerStoreService.Awaiting(storeService => storeService.UpdateProductAsync(updateProduct));

            // Assert
            await result.Should().ThrowAsync<UnAuthorizedException>();
            var updatedProduct = await _ownerStoreService.GetStoreProductAsync(updateProduct.Guid);
            updatedProduct.Should()
                .NotBeEquivalentTo(updateProduct, options => options.Excluding(product => product.Rating));
        }

        #endregion

        #region OwnerNomination

        [Fact]
        public async Task
            NominateStoreOwnerAsync_StoreOwnerSuccessfullyAdded_WhenNewOwnerIsValidAndNotPreviouslyNominated()
        {
            // Arrange
            var newOwner = _fixture
                .Build<StoreOwnershipDto>()
                .With(ownership => ownership.User, new UserDto { Guid = _notOwnerUser })
                .With(ownership => ownership.Store, _storeOwnership.Store)
                .Without(ownership => ownership.Guid)
                .Create();

            // Act
            var result = await _ownerStoreService.NominateNewStoreOwnerAsync(_storeOwnership.Guid, newOwner);
            var addedOwner = await _ownerStoreService.GetStoreOwnerShipAsync(newOwner.User.Guid, newOwner.Store.Guid);


            // Assert
            result.Should().BeTrue();
            addedOwner.Should().BeEquivalentTo(newOwner, options => options
                .Excluding(ownership => ownership.Guid)
                .Excluding(management => management.User.Notifications));
        }

        [Fact]
        public async Task NominateStoreOwnerAsync_StoreOwnerNotAdded_WhenNewOwnerIsAlreadyASeller()
        {
            // Arrange
            var newOwner = _fixture
                .Build<StoreOwnershipDto>()
                .With(ownership => ownership.User, new UserDto { Guid = _notOwnerUser })
                .With(ownership => ownership.Store, _storeOwnership.Store)
                .Without(ownership => ownership.Guid)
                .Create();

            await _ownerStoreService.NominateNewStoreOwnerAsync(_storeOwnership.Guid, newOwner);

            // Act
            var result = await _ownerStoreService.NominateNewStoreOwnerAsync(_storeOwnership.Guid, newOwner);
            newOwner = await _ownerStoreService.GetStoreOwnerShipAsync(newOwner.User.Guid, newOwner.Store.Guid);
            var sellers = await _ownerStoreService.GetSubordinateSellersAsync(newOwner.Guid);
            sellers.Should().BeEquivalentTo(new StoreSellersDto());
            result.Should().BeFalse();
        }

        [Fact]
        public async Task NominateStoreOwnerAsync_ThrowsAuthorizationException_WhenNominatorIsNotAnOwner()
        {
            // Arrange
            var newOwner = _fixture
                .Build<StoreOwnershipDto>()
                .With(ownership => ownership.User, new UserDto { Guid = _notOwnerUser })
                .With(ownership => ownership.Store, _storeOwnership.Store)
                .Without(ownership => ownership.Guid)
                .Create();

            var result = _notOwnerStoreService.Awaiting(storeService =>
                storeService.NominateNewStoreOwnerAsync(_notOwnerUser, newOwner));

            await result.Should().ThrowAsync<UnAuthorizedException>();
        }

        [Fact(Skip = "Test fails because a fix is required for the function.")]
        public async Task NominateStoreOwnerAsync_StoreOwnerNotAdded_WhenNominatedIsNotRegistered()
        {
            // Arrange
            var newOwner = _fixture
                .Build<StoreOwnershipDto>()
                .With(ownership => ownership.Store, _storeOwnership.Store)
                .Without(ownership => ownership.Guid)
                .Create();

            var result = await _ownerStoreService.NominateNewStoreOwnerAsync(_storeOwnership.User.Guid, newOwner);
            var addedOwner = await _ownerStoreService.GetStoreOwnerShipAsync(newOwner.User.Guid, newOwner.Store.Guid);

            result.Should().BeFalse();
            addedOwner.Should().BeNull();
        }

        #endregion

        #region ManagerNomination

        [Fact]
        public async Task
            NominateStoreManagerAsync_StoreManagerSuccessfullyAdded_WhenNewManagerIsValidAndNotPreviouslyNominated()
        {
            // Arrange
            var newManager = _fixture
                .Build<StoreManagementDto>()
                .With(management => management.User, new UserDto {Guid = _notOwnerUser})
                .With(management => management.Store, _storeOwnership.Store)
                .Without(management => management.Guid)
                .Create();

            // Act
            var result = await _ownerStoreService.NominateNewStoreManagerAsync(_storeOwnership.Guid, newManager);
            var addedManager = await _ownerStoreService.GetStoreManagementAsync(newManager.User.Guid, newManager.Store.Guid);

            // Assert
            result.Should().BeTrue();
            addedManager.Should()
                .BeEquivalentTo(newManager, options => options
                    .Excluding(management => management.Guid)
                    .Excluding(management => management.User.Notifications));
        }

        [Fact]
        public async Task NominateStoreManagerAsync_StoreManagerNotAdded_WhenNewManagerIsAlreadyASeller()
        {
            // Arrange
            var newOwner = _fixture
                .Build<StoreOwnershipDto>()
                .With(ownership => ownership.User, new UserDto {Guid = _notOwnerUser})
                .With(ownership => ownership.Store, _storeOwnership.Store)
                .Without(ownership => ownership.Guid)
                .Create();

            var newManager = _fixture
                .Build<StoreManagementDto>()
                .With(management => management.User, newOwner.User)
                .With(management => management.Store, newOwner.Store)
                .Without(management => management.Guid)
                .Create();

            await _ownerStoreService.NominateNewStoreOwnerAsync(_storeOwnership.Guid, newOwner);
            newOwner = await _ownerStoreService.GetStoreOwnerShipAsync(newOwner.User.Guid, newOwner.Store.Guid);

            // Act
            var result = await _ownerStoreService.NominateNewStoreManagerAsync(_storeOwnership.Guid, newManager);
            newOwner = await _ownerStoreService.GetStoreOwnerShipAsync(newOwner.User.Guid, newOwner.Store.Guid);
            var sellers = await _ownerStoreService.GetSubordinateSellersAsync(newOwner.Guid);
            sellers.Should().BeEquivalentTo(new StoreSellersDto());
            result.Should().BeFalse();
        }

        [Fact]
        public async Task NominateStoreManagerAsync_ThrowsAuthorizationException_WhenNominatorIsNotAManager()
        {
            // Arrange
            var newManager = _fixture
                .Build<StoreManagementDto>()
                .With(management => management.User, new UserDto { Guid = _notOwnerUser })
                .With(management => management.Store, _storeOwnership.Store)
                .Without(management => management.Guid)
                .Create();

            var result = _notOwnerStoreService.Awaiting(storeService =>
                storeService.NominateNewStoreManagerAsync(_notOwnerUser, newManager));

            await result.Should().ThrowAsync<UnAuthorizedException>();
        }

        [Fact(Skip = "Test fails because a fix is required for the function.")]
        public async Task NominateStoreManagerAsync_StoreManagerNotAdded_WhenNominatedIsNotRegistered()
        {
            // Arrange
            var newManager = _fixture
                .Build<StoreManagementDto>()
                .With(management => management.Store, _storeOwnership.Store)
                .Without(management => management.Guid)
                .Create();

            var result = await _ownerStoreService.NominateNewStoreManagerAsync(_storeOwnership.Guid, newManager);
            var addedManager = await _ownerStoreService.GetStoreManagementAsync(newManager.User.Guid, newManager.Store.Guid);

            result.Should().BeFalse();
            addedManager.Should().BeNull();
        }

        #endregion

        #region ManagerPermissions

        [Fact]
        public async Task UpdateStoreManagerPermissionsAsync_ShouldSuccessfullyUpdate_WhenPermissionsAreValid()
        {
            // Arrange
            var newManager = _fixture
                .Build<StoreManagementDto>()
                .With(management => management.Store, _storeOwnership.Store)
                .Without(management => management.Guid)
                .Without(management => management.Permissions)
                .Create();

            await _ownerStoreService.NominateNewStoreManagerAsync(_storeOwnership.Guid, newManager);
            newManager = await _ownerStoreService.GetStoreManagementAsync(newManager.User.Guid, newManager.Store.Guid);
            var newPermissions = new StoreManagementPermissionsDto
            {
                Guid = newManager.Permissions.Guid,
                CanGetSellersInfo = false,
                CanAddProduct = true,
                CanDeleteProduct = true,
                CanUpdateProduct = false
            };
            
            //Act
            await _ownerStoreService.UpdateManagerPermissionAsync(newPermissions);
            var managerWithNewPermissions = await _ownerStoreService.GetStoreManagementAsync(newManager.Guid);

            // Assert
            managerWithNewPermissions.Permissions.Should().BeEquivalentTo(newPermissions);
        }
        [Fact]
        public async Task UpdateStoreManagerPermissionsAsync_ShouldThrowValidationException_WhenPermissionsAreNotValid()
        {
            // Arrange
            var newManager = _fixture
                .Build<StoreManagementDto>()
                .With(management => management.Store, _storeOwnership.Store)
                .Without(management => management.Guid)
                .Create();

            await _ownerStoreService.NominateNewStoreManagerAsync(_storeOwnership.Guid, newManager);
            var newPermissions = new StoreManagementPermissionsDto
            {
                CanGetSellersInfo = false,
                CanAddProduct = true,
                CanDeleteProduct = true,
                CanUpdateProduct = false
            };

            // Act
            var action = _ownerStoreService.Awaiting(service => service.UpdateManagerPermissionAsync(newPermissions));

            // Assert
            await action.Should().ThrowAsync<ValidationException>();
        }
        #endregion

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        [Fact]
        public async Task RemoveManager_RemoveManagerSuccessfully_WhenUserIsStoreOwner()
        {
            // Arrange
            var fixtureManager = _fixture
                .Build<StoreManagementDto>()
                .With(s => s.User, new UserDto {Guid = _notOwnerUser})
                .With(s => s.Store, _storeOwnership.Store)
                .Without(s => s.Guid)
                .Create();


            await _ownerStoreService.NominateNewStoreManagerAsync(_storeOwnership.Guid, fixtureManager);
            var managerToRemove = await _ownerStoreService.GetStoreManagementAsync(
                _notOwnerUser, _storeOwnership.Store.Guid);

            // Act
            var result = await _ownerStoreService.RemoveManagerAsync(_storeOwnership.Guid, managerToRemove.Guid);

            // Assert
            result.Should().BeTrue();

            var manager =
                await _ownerStoreService.GetStoreManagementAsync(_storeOwnership.User.Guid, _storeOwnership.Store.Guid);
            var ownerNominatedManagerList = (await _ownerStoreService.GetSubordinateSellersAsync(_storeOwnership.Guid));
            var managers = ownerNominatedManagerList.StoreManagers;
            manager.Should().BeNull();
            managers.Should().BeEmpty();
        }

        [Fact]
        public async Task RemoveManager_RemoveManagerUnSuccessfully_WhenUserIsNotManager()
        {
            // Arrange

            var managerToRemove = System.Guid.NewGuid();

            // Act
            var result = await _ownerStoreService.RemoveManagerAsync(_storeOwnership.Guid, managerToRemove);

            // Assert
            result.Should().BeFalse();

            var manager =
                await _ownerStoreService.GetStoreManagementAsync(_storeOwnership.User.Guid, _storeOwnership.Store.Guid);
            manager.Should().BeNull();

        }
        [Fact]
        public async Task GetAllSellersInformation_ReturnsListOfSellersInformation_WhenUserIsStoreOwner()
        {
            // Arrange

            var storeGuid = _storeOwnership.Store;

            // Act
            var result = await _ownerStoreService.GetAllSellersInformationAsync(storeGuid.Guid);

            // Assert
            result.Should().NotBeNull();

            var owners = result.StoreOwners;
            owners.Should().NotBeEmpty();
            var owner = owners.First();

            owner.Guid.Should().Be(_storeOwnership.Guid);

        }
        [Fact]
        public async Task GetAllSellersInformation_ReturnsNull_WhenUserIsNotAnOwner()
        {
            // Arrange

            var store = _storeOwnership.Store;

            // Act
            var result =  _notOwnerStoreService.Awaiting(storeService => storeService.GetAllSellersInformationAsync(store.Guid));

            // Assert
             await result.Should().ThrowAsync<UnAuthorizedException>();

        }
        
        [Fact]
        public async Task GetStorePurchaseHistory_ReturnStorePurchaseHistory_WhenUserIsTheStoreOwner()
        {
            // Arrange
            var store = _storeOwnership.Store;

            // Act
            var result = await _ownerStoreService.GetStorePurchaseHistoryAsync(store.Guid);
            var storePurchase = result.First();
            var purchaseProduct = storePurchase.PurchaseProducts.First();
            var realStorePurchase = _purchase.StorePurchases.First();
            var realPurchaseProduct = realStorePurchase.PurchaseProducts.First();

            // Assert
            storePurchase.Should().BeEquivalentTo(realStorePurchase, opt => opt.Excluding(p => p.Guid).Excluding(p => p.PurchaseProducts));
            purchaseProduct.Should().BeEquivalentTo(realPurchaseProduct, opt => opt.Excluding(p => p.Guid).Excluding(p => p.ProductGuid));
        }
        
        [Fact]
        public async Task GetStorePurchaseHistory_ReturnStorePurchaseHistory_WhenUserIsNotAnOwner()
        {
            // Arrange

            var store = _storeOwnership.Store;

            // Act
            var result =  _notOwnerStoreService.Awaiting(storeService => storeService.GetStorePurchaseHistoryAsync(store.Guid));

            // Assert
            await result.Should().ThrowAsync<UnAuthorizedException>();
        }
        
    }
}