using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using BoomaEcommerce.Core.Exceptions;
using BoomaEcommerce.Services.Authentication;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Stores;
using BoomaEcommerce.Services.Tests;
using BoomaEcommerce.Tests.CoreLib;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Xunit;

namespace BoomaEcommerce.AcceptanceTests
{
    public class OwnerAcceptanceTests : IAsyncLifetime
    {
        private IStoresService _ownerStoreService;
        private StoreOwnershipDto _storeOwnership;

        private UserDto _notOwnerUser;
        private IStoresService _notOwnerStoreService;

        private IFixture _fixture;

        public async Task InitializeAsync()
        {

            _fixture = new Fixture();
            _fixture.Customize<StoreDto>(s => s.Without(ss => ss.Guid));

            var serviceMockFactory = new ServiceMockFactory(); 
            var storeService = serviceMockFactory.MockStoreService();
            var authService = serviceMockFactory.MockAuthenticationService();
            await InitOwnerUser(storeService, authService);
            await InitNotOwnerUser(storeService, authService);

            _fixture.Customize<ProductDto>(p => p.Without(pp => pp.Guid).Without(pp => pp.Rating)
                .With(pp => pp.Store, _storeOwnership.Store));
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

        private async Task InitNotOwnerUser(IStoresService storeService, IAuthenticationService authService)
        {
            const string notOwnerUsername = "Ori";
            const string notOwnerPassword = "Ori1234";
            await authService.RegisterAsync(notOwnerUsername, notOwnerPassword);
            var notOwnerLoginResponse = await authService.LoginAsync(notOwnerUsername, notOwnerPassword);
            _notOwnerUser = notOwnerLoginResponse.User;
            var result = SecuredStoreService.CreateSecuredStoreService(notOwnerLoginResponse.Token,
                ServiceMockFactory.Secret, storeService, out _notOwnerStoreService);

            if (!result)
            {
                throw new Exception("This shouldn't happen");
            }
        }

        [Fact]
        public async Task CreateStoreProductAsync_ReturnsTrueAndCreatesProduct_WhenUserIsStoreOwner()
        {
            // Arrange
            var productDto = _fixture.Create<ProductDto>();

            // Act
            var result = await _ownerStoreService.CreateStoreProductAsync(productDto);
            // Assert
            result.Should().BeEquivalentTo(productDto, opt => opt.Excluding(p => p.Guid).Excluding(p => p.Rating));
            var product = await _ownerStoreService.GetStoreProduct(result.Guid);
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
            var notActuallyRemovedProduct = await _notOwnerStoreService.GetStoreProduct(resultProduct.Guid);
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
            var removedProduct = await _ownerStoreService.GetStoreProduct(resultProduct.Guid);
            result.Should().BeTrue();
            removedProduct.Should().BeNull();
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }
    }
}