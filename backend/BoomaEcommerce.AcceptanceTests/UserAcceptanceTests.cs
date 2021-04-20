using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using BoomaEcommerce.Services.Authentication;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Stores;
using BoomaEcommerce.Tests.CoreLib;
using FluentAssertions;
using Xunit;

namespace BoomaEcommerce.AcceptanceTests
{
    public class UserAcceptanceTests : IAsyncLifetime
    {
        private IStoresService _storeService;
        private IFixture _fixture;
        private UserDto user; 

        public async Task InitializeAsync()
        {
            _fixture = new Fixture();
            _fixture.Customize<StoreDto>(s =>
                s.Without(ss => ss.Guid).Without(ss => ss.Rating));

            var serviceMockFactory = new ServiceMockFactory();

            var storeService = serviceMockFactory.MockStoreService();
            var authService = serviceMockFactory.MockAuthenticationService();
            await InitUser(storeService, authService);
        }

        private async Task InitUser(IStoresService storeService, IAuthenticationService authService)
        {
            const string username = "Omer";
            const string password = "Omer1001";

            await authService.RegisterAsync(username, password);
            var loginResponse = await authService.LoginAsync(username, password);

            user = _fixture
             .Build<UserDto>()
             .With(s => s.UserName ,loginResponse.User.UserName)
             .Create();

            var createServiceResult = SecuredStoreService.CreateSecuredStoreService(loginResponse.Token,
                 ServiceMockFactory.Secret, storeService, out _storeService);
            if (!createServiceResult)
            {
                throw new Exception("This shouldn't happen");
            }
        }


        [Fact]
        public async Task CreateStore_ShouldAddStore_WhenDetailsAreValid()
        {
            //Arrange
            var fixtureStore = _fixture
                .Build<StoreDto>()
                .With(s => s.StoreFounder, user )
                .Without(s => s.Rating)
                .Without(s => s.Guid)
                .Create();
            //Act 
            await _storeService.CreateStoreAsync(fixtureStore);

            //Assert 
            var stores = await _storeService.GetStoresAsync();
            var store = stores.First();

            store.Description.Should().BeEquivalentTo(fixtureStore.Description);
            store.StoreFounder.Should().BeEquivalentTo(fixtureStore.StoreFounder);
            store.StoreName.Should().BeEquivalentTo(fixtureStore.StoreName);  
           
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }



    }
}
