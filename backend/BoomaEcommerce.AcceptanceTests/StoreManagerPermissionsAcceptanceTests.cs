using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using BoomaEcommerce.Core.Exceptions;
using BoomaEcommerce.Services.Authentication;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Stores;
using BoomaEcommerce.Tests.CoreLib;
using FluentAssertions;
using Xunit;

namespace BoomaEcommerce.AcceptanceTests
{
    
    public class StoreManagerPermissionsAcceptanceTests : IAsyncLifetime
    {
        private StoreManagementDto _storeManagementWithPermissions;
        private IStoresService _managerStoreServiceWithPermissions;
        
        private StoreManagementDto _storeManagementWithoutPermissions;
        private IStoresService _managerStoreServiceWithoutPermissions;
        
        private IStoresService _ownerStoreService;
        private StoreOwnershipDto _storeOwnership;
        
        private IFixture _fixture;

        public async Task InitializeAsync()
        {
            _fixture = new Fixture();

            var serviceMockFactory = new ServiceMockFactory();
            
            var storeService = serviceMockFactory.MockStoreService();
            var authService = serviceMockFactory.MockAuthenticationService();
            await InitOwnerUser(storeService, authService);
            await InitManagerUserWithPermissions(storeService, authService);
            await InitManagerUserWithoutPermissions(storeService, authService);
            _fixture.Customize<ProductDto>(
                p => p
                    .With(p => p.Store, _storeOwnership.Store)
                    .With(pp => pp.Amount, 10)
                    .With(pp => pp.Price, 10)
                    .Without(pp => pp.Rating)
                    .Without(pp => pp.Guid));
        }

        private async Task InitManagerUserWithoutPermissions(IStoresService storeService, IAuthenticationService authService)
        {
            const string username = "Matan";
            const string password = "Matan1234";
            
            await authService.RegisterAsync(username, password);
            var loginResponse = await authService.LoginAsync(username, password);

            var fixtureStoreManagementPermissions = _fixture
                .Build<StoreManagementPermissionDto>()
                .With(sm => sm.CanAddProduct, false)
                .With(sm => sm.CanDeleteProduct, false)
                .With(sm => sm.CanUpdateProduct, false)
                .With(sm => sm.CanUpdateProduct, false)
                .Without(sm => sm.Guid)
                .Create();

            var fixtureStoreManagement = _fixture
                .Build<StoreManagementDto>()
                .With(sm => sm.Permissions, fixtureStoreManagementPermissions)
                .With(sm => sm.Store, _storeOwnership.Store)
                .With(sm => sm.User, loginResponse.User)
                .Without(sm => sm.Guid)
                .Create();

            var nominateResult = await _ownerStoreService.NominateNewStoreManager(_storeOwnership.User.Guid, fixtureStoreManagement);
            if (!nominateResult)
            {
                throw new Exception("This shouldn't happen");
            }

            _storeManagementWithoutPermissions = (await storeService.GetAllStoreManagements(loginResponse.User.Guid)).First();
            var createServiceResult = SecuredStoreService.CreateSecuredStoreService(loginResponse.Token,
                ServiceMockFactory.Secret, storeService, out _managerStoreServiceWithoutPermissions);
            if (!createServiceResult)
            {
                throw new Exception("This shouldn't happen");
            }
        }

        private async Task InitManagerUserWithPermissions(IStoresService storeService, IAuthenticationService authService)
        {
            const string username = "Ori";
            const string password = "Ori1234";
            
            await authService.RegisterAsync(username, password);
            var loginResponse = await authService.LoginAsync(username, password);

            var fixtureStoreManagementPermissions = _fixture
                .Build<StoreManagementPermissionDto>()
                .With(sm => sm.CanAddProduct, true)
                .With(sm => sm.CanDeleteProduct, true)
                .With(sm => sm.CanUpdateProduct, true)
                .With(sm => sm.CanUpdateProduct, true)
                .Without(sm => sm.Guid)
                .Create();

            var fixtureStoreManagement = _fixture
                .Build<StoreManagementDto>()
                .With(sm => sm.Permissions, fixtureStoreManagementPermissions)
                .With(sm => sm.Store, _storeOwnership.Store)
                .With(sm => sm.User, loginResponse.User)
                .Without(sm => sm.Guid)
                .Create();

            var nominateResult = await _ownerStoreService.NominateNewStoreManager(_storeOwnership.User.Guid, fixtureStoreManagement);
            if (!nominateResult)
            {
                throw new Exception("This shouldn't happen");
            }

            _storeManagementWithPermissions = (await storeService.GetAllStoreManagements(loginResponse.User.Guid)).First();
            var createServiceResult = SecuredStoreService.CreateSecuredStoreService(loginResponse.Token,
                ServiceMockFactory.Secret, storeService, out _managerStoreServiceWithPermissions);
            if (!createServiceResult)
            {
                throw new Exception("This shouldn't happen");
            }
        }

        private async Task InitOwnerUser(IStoresService storeService, IAuthenticationService authService)
        {
            const string username = "Arik";
            const string password = "Arik1337";

            await authService.RegisterAsync(username, password);
            var loginResponse = await authService.LoginAsync(username, password);

            var fixtureStore = _fixture
                .Build<StoreDto>()
                .With(s => s.StoreFounder, loginResponse.User)
                .Without(s => s.Rating)
                .Without(s => s.Guid)
                .Create();

            await storeService.CreateStoreAsync(fixtureStore);


            _storeOwnership = (await storeService.GetAllStoreOwnerShips(loginResponse.User.Guid)).First();
            var result = SecuredStoreService.CreateSecuredStoreService(loginResponse.Token,
                ServiceMockFactory.Secret, storeService, out _ownerStoreService);
            if (!result)
            {
                throw new Exception("This shouldn't happen");
            }
        }
        
        [Fact]
        public async Task CreateStoreProductAsync_ReturnsProduct_WhenManagerHasAddProductPermission()
        {
            // Arrange
            var fixtureProductDto = _fixture.Create<ProductDto>();
            // Act
            var result = await _managerStoreServiceWithPermissions.CreateStoreProductAsync(fixtureProductDto);
            // Assert
            result.Should().NotBeNull().And.BeEquivalentTo(fixtureProductDto, 
                opt => opt.Excluding(p => p.Guid).Excluding(p => p.Rating));
        }
        
        [Fact]
        public async Task CreateStoreProductAsync_ThrowsUnAuthorizedException_WhenManagerDoesNotHaveAddProductPermission()
        {
            // Arrange
            var fixtureProductDto = _fixture.Create<ProductDto>();
            // Act
            var act = _managerStoreServiceWithoutPermissions.Awaiting(service =>
                service.CreateStoreProductAsync(fixtureProductDto));
            // Assert
            await act.Should().ThrowAsync<UnAuthorizedException>();
        }
        
        [Fact]
        public async Task DeleteProductAsync_ReturnsTrueAndDeletesProduct_WhenManagerHasDeleteProductPermission()
        {
            // Arrange
            var fixtureProductDto = _fixture.Create<ProductDto>();
            var productDto = await _ownerStoreService.CreateStoreProductAsync(fixtureProductDto);
            // Act
            var result = await _managerStoreServiceWithPermissions.DeleteProductAsync(productDto.Guid);
            // Assert
            result.Should().BeTrue();
            var product = await _ownerStoreService.GetStoreProduct(productDto.Guid);
            product.Should().BeNull();

        }
        
        [Fact]
        public async Task DeleteProductAsync_ThrowsUnAuthorizedException_WhenManagerDoesNotHaveDeleteProductPermission()
        {
            // Arrange
            var fixtureProductDto = _fixture.Create<ProductDto>();
            var productDto = await _ownerStoreService.CreateStoreProductAsync(fixtureProductDto);
            // Act
            var act = _managerStoreServiceWithoutPermissions.Awaiting(service =>
                service.DeleteProductAsync(productDto.Guid));
            // Assert
            await act.Should().ThrowAsync<UnAuthorizedException>();
        }
        
        [Fact]
        public async Task UpdateProductAsync_ReturnsTrueAndUpdatesProduct_WhenManagerHasUpdateProductPermission()
        {
            // Arrange
            var fixtureProductDto = _fixture.Create<ProductDto>();
            var productDto = await _ownerStoreService.CreateStoreProductAsync(fixtureProductDto);
            var updatedProductDto = _fixture
                .Build<ProductDto>()
                .With(p => p.Store, _storeOwnership.Store)
                .With(p => p.Guid, productDto.Guid)
                .With(p => p.Amount, 20)
                .With(p => p.Rating, 5)
                .Without(p => p.Category)
                .Without(p => p.Price)
                .Without(p => p.Name)
                .Create();
            // Act
            var result = await _managerStoreServiceWithPermissions.UpdateProductAsync(updatedProductDto);
            // Assert
            result.Should().BeTrue();
            var product = await _ownerStoreService.GetStoreProduct(productDto.Guid);
            product.Should().BeEquivalentTo(productDto,
                opt => opt
                    .Excluding(p => p.Guid)
                    .Excluding(p => p.Amount)
                    .Excluding(p => p.Rating)); 
            product.Amount.Should().Be(20);
            product.Rating.Should().Be(5);
        }
        
        [Fact]
        public async Task UpdateProductAsync_ThrowsUnAuthorizedException_WhenManagerDoesNotHaveUpdateProductPermission()
        {
            // Arrange
            var fixtureProductDto = _fixture.Create<ProductDto>();
            var productDto = await _ownerStoreService.CreateStoreProductAsync(fixtureProductDto);
            var updatedProductDto = _fixture
                .Build<ProductDto>()
                .With(p => p.Store, _storeOwnership.Store)
                .With(p => p.Guid, productDto.Guid)
                .With(p => p.Amount, 20)
                .With(p => p.Rating, 5)
                .Without(p => p.Category)
                .Without(p => p.Price)
                .Without(p => p.Name)
                .Create();
            // Act
            var act = _managerStoreServiceWithoutPermissions.Awaiting(service =>
                service.UpdateProductAsync(updatedProductDto));
            // Assert
            await act.Should().ThrowAsync<UnAuthorizedException>();
        }
        
        [Fact]
        public async Task GetAllSellersInformation_ReturnsAllSellersInformation_WhenManagerHasSellerInformationPermission()
        {
            // Arrange
            var storeGuid = _storeManagementWithPermissions.Store.Guid;
            // Act
            var result = await _managerStoreServiceWithPermissions
                .GetAllSellersInformation(storeGuid);
            // Assert
            result.Should().NotBeNull();
            result.StoreManagers.Exists(sm => 
                sm.User.Guid == _storeManagementWithoutPermissions.User.Guid).Should().BeTrue();
            result.StoreManagers.Exists(sm => 
                sm.User.Guid == _storeManagementWithPermissions.User.Guid).Should().BeTrue();
            result.StoreOwners.Exists(sw => sw.User.Guid == _storeOwnership.User.Guid);
        }
        
        [Fact]
        public async Task GetAllSellersInformation_ThrowsUnAuthorizedException_WhenManagerDoesNotHaveSellerInformationPermission()
        {
            // Arrange
            var storeGuid = _storeManagementWithPermissions.Store.Guid;
            // Act
            var act = _managerStoreServiceWithoutPermissions.Awaiting(service =>
                service.GetAllSellersInformation(storeGuid));
            // Assert
            await act.Should().ThrowAsync<UnAuthorizedException>();
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }
    }
}