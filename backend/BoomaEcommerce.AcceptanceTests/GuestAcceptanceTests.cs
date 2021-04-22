using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AutoFixture;
using BoomaEcommerce.Core.Exceptions;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.Authentication;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Products;
using BoomaEcommerce.Services.Stores;
using BoomaEcommerce.Services.Tests;
using BoomaEcommerce.Services.Users;
using BoomaEcommerce.Tests.CoreLib;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Xunit;

namespace BoomaEcommerce.AcceptanceTests
{
    public class GuestAcceptanceTests : IAsyncLifetime
    {
        private IFixture _fixture;

        private IAuthenticationService authService;

        private IStoresService storesServiceWithData;

        private IProductsService productsService;

        private IUsersService usersService;
        

        public async Task InitializeAsync()
        {
            _fixture = new Fixture();

            var serviceMockFactory = new ServiceMockFactory();
            storesServiceWithData = serviceMockFactory.MockStoreService();
            authService = serviceMockFactory.MockAuthenticationService();
            productsService = serviceMockFactory.MockProductService();
            usersService = serviceMockFactory.MockUserService();
            await InitStoreWithData(storesServiceWithData, authService);
        }

        private async Task InitStoreWithData(IStoresService storesService, IAuthenticationService authService)
        {
            const string username = "BennyOwner";
            const string password = "superSecretOwnerPass";

            await authService.RegisterAsync(username, password);
            var loginResult = await authService.LoginAsync(username, password);

            var fixtureStore = _fixture
                .Build<StoreDto>()
                .With(s => s.StoreFounder, loginResult.User)
                .With(s => s.Description, "myStore")
                .Without(s => s.Rating)
                .Without(s => s.Guid)
                .Create();

            await storesService.CreateStoreAsync(fixtureStore);

            var myStore = (await storesService.GetStoresAsync()).First();

            var p1Dto = _fixture
                .Build<ProductDto>()
                .With(p => p.Store, myStore)
                .With(p => p.Amount, 20)
                .With(p => p.Category, "Toiletry")
                .Without(p => p.Rating)
                .With(p => p.Price, 1)
                .Without(p => p.Name)
                .Create();

            await storesService.CreateStoreProductAsync(p1Dto);

            var p2Dto = _fixture
                .Build<ProductDto>()
                .With(p => p.Store, myStore)
                .With(p => p.Amount, 10)
                .With(p => p.Category, "Diary")
                .Without(p => p.Rating)
                .With(p => p.Price, 1)
                .Without(p => p.Name)
                .Create();

            await storesService.CreateStoreProductAsync(p2Dto);

            storesServiceWithData = new SecuredStoreService(storesService);
        }
        [Fact]
        public async Task RegisterAsync_ReturnsSuccessfulAuthenticationResult_WhenUsernameDoesNotExist()
        {
            // Arrange 
            const string username = "guest";
            const string password = "guestIsTheBest";

            // Act 
            var registerResult =  await authService.RegisterAsync(username, password);

            // Assert
            registerResult.Success.Should().BeTrue();
        }
        [Fact]
        public async Task RegisterAsync_ReturnsUnsuccessfulAuthenticationResult_WhenUsernameAlreadyExists()
        {
            // Arrange 
            const string usernameGood = "guest";
            const string passwordGood = "guestIsTheBest";
            const string usernameDupBad = "guest";
            const string passwordDupBad = "IAmTheBestGuest";

            // Act 
            var registerResultOk = await authService.RegisterAsync(usernameGood, passwordGood);
            var registerResultBad = await authService.RegisterAsync(usernameDupBad, passwordDupBad);

            // Assert
            registerResultOk.Success.Should().BeTrue();
            registerResultBad.Success.Should().BeFalse();
        }
        [Fact]
        public async Task LoginAsync_ReturnsSuccessfulAuthenticationResult_WhenUserIsAlreadyRegistered()
        {
            // Arrange 
            const string username = "guest";
            const string password = "guestIsTheBest";

            // Act
            await authService.RegisterAsync(username, password);
            var loginResult = await authService.LoginAsync(username, password);

            // Assert
            loginResult.Success.Should().BeTrue();
        }
        
        /*[Fact]
        public async Task LoginAsync_ReturnsUnsuccessfulAuthenticationResult_WhenUserHasNotRegistered()
        {
            // Arrange 
            const string username = "guest";
            const string password = "guestIsTheBest";

            // Act 
            await authService.RegisterAsync(username, password);
            var badPasswordLogin = await authService.LoginAsync(username, "another password");
            var badUsernameLogin = await authService.LoginAsync("guesty", password);

            // Assert
            badUsernameLogin.Success.Should().BeFalse();
            badPasswordLogin.Success.Should().BeFalse();
        }*/
        [Fact]
        public async Task GetStoreData__ReturnsCompatibleStoreData_WhenStoreExists()
        { 

            // Act 
            var storeData = await storesServiceWithData.GetStoresAsync();
            StoreDto myStore = storeData.First();
            

            // Assert
            myStore.StoreFounder.UserName.Should().BeEquivalentTo("BennyOwner");
            myStore.Description.Should().BeEquivalentTo("myStore");
        }

        [Fact]
        public async Task GetStoreData__ReturnsNull_WhenStoreDoesNotExist()
        {

            // Act 
            var storeData = await storesServiceWithData.GetStoreAsync(Guid.NewGuid());

            // Assert
            storeData.Should().BeNull();
        }
        
        [Fact]
        public async Task GetProductsFromStoreAsync_ReturnProperProductsListOfStore_whenStoreExist()
        {
            // Act 
            var storeData = await storesServiceWithData.GetStoresAsync();
            StoreDto myStore = storeData.First();
            var products = await storesServiceWithData.GetProductsFromStoreAsync(myStore.Guid);

            // Assert
            products.Count.Should().Be(2);
        }

        [Fact]
        public async Task GetProductByCategory_ReturnsProductsOfCategory_WhenCategoryProductsExist()
        {
            var toiletryProducts = (await productsService.GetProductByCategoryAsync("Toiletry")).First();
            var diaryProducts = (await productsService.GetProductByCategoryAsync("Diary")).First();
            toiletryProducts.Amount.Should().Be(20);
            diaryProducts.Amount.Should().Be(10);
        }

        [Fact]
        public async Task GetProductByCategory_ReturnsEmptyList_WhenCategoryProductsDoNotExist()
        {
            // Act 
            var toiletryProducts = (await productsService.GetProductByCategoryAsync("SomeCategory"));
            // Assert
            toiletryProducts.Should().BeEmpty();
        }

       


        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
