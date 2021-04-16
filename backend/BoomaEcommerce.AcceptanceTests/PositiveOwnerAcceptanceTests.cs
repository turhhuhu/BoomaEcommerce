using System;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Stores;
using BoomaEcommerce.Services.Tests;
using BoomaEcommerce.Tests.CoreLib;
using FluentAssertions;
using Xunit;

namespace BoomaEcommerce.AcceptanceTests
{
    public class PositiveOwnerAcceptanceTests : IAsyncLifetime
    {
        private IStoresService _storesService;
        private StoreOwnershipDto _storeOwnership;
        public async Task InitializeAsync()
        {
            var storeService = ServiceMockFactory.MockStoreService();
            var authService = ServiceMockFactory.MockAuthenticationService();
            const string username = "Arik";
            const string password = "Arik1337";
            await authService.RegisterAsync(username, password);
            var loginResponse = await authService.LoginAsync(username, password);
            await storeService.CreateStoreAsync(new StoreDto {StoreFounder = loginResponse.User});
            _storeOwnership = (await storeService.GetAllStoreOwnerShips(loginResponse.User.Guid)).First();
            var result = SecuredStoreService.CreateSecuredStoreService(loginResponse.Token,
                ServiceMockFactory.Secret, storeService, out _storesService);
            if (!result)
            {
                throw new Exception("This shouldn't happen");
            }
        }

        [Fact]
        public async Task CreateStoreProductAsync_ReturnsTrueAndCreatesProduct_WhenUserIsStoreOwner()
        {
            // Arrange
            var productDto = new ProductDto
                {Amount = 10, Category = "Chess", Price = 10, Rating = "3", Store = _storeOwnership.Store};
            // Act
            var result = await _storesService.CreateStoreProductAsync(productDto);
            // Assert
            result.Should().NotBeNull();
            var product = await _storesService.GetStoreProduct(productDto.Guid);
            product.Should().NotBeNull();
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }
    }
}