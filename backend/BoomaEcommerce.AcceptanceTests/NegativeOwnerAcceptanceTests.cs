using System;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Core.Exceptions;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Stores;
using BoomaEcommerce.Tests.CoreLib;
using FluentAssertions;
using Xunit;

namespace BoomaEcommerce.AcceptanceTests
{
    public class NegativeOwnerAcceptanceTests : IAsyncLifetime
    {
        private IStoresService _storesService;
        private StoreOwnershipDto _storeOwnership;
        private UserDto _notOwnerUser;
        public async Task InitializeAsync()
        {
            var storeService = ServiceMockFactory.MockStoreService();
            var authService = ServiceMockFactory.MockAuthenticationService();
            const string ownerUsername = "Arik";
            const string ownerPassword = "Arik1337";
            const string notOwnerUsername = "Ori";
            const string notOwnerPassword = "Ori1234";
            await authService.RegisterAsync(ownerUsername, ownerPassword);
            var ownerLoginResponse = await authService.LoginAsync(ownerUsername, ownerPassword);
            await storeService.CreateStoreAsync(new StoreDto {StoreFounder = ownerLoginResponse.User});
            _storeOwnership = (await storeService.GetAllStoreOwnerShips(ownerLoginResponse.User.Guid)).First();
            await authService.RegisterAsync(notOwnerUsername, notOwnerPassword);
            var notOwnerLoginResponse = await authService.LoginAsync(notOwnerUsername, notOwnerPassword);
            _notOwnerUser = notOwnerLoginResponse.User;
            var result = SecuredStoreService.CreateSecuredStoreService(notOwnerLoginResponse.Token,
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
            var act = _storesService.Awaiting(x => x.CreateStoreProductAsync(productDto));
            // Assert
            await act.Should().ThrowAsync<UnAuthorizedException>();

        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }
    }
}