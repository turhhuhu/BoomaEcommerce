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

        private StoreDto myStore;
        private ProductDto p1Dto;
        private ProductDto p2Dto;

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
            var user = new UserDto {UserName = "BennyOwner"};
            const string password = "superSecretOwnerPass";

            await authService.RegisterAsync(user, password);
            var loginResult = await authService.LoginAsync(user.UserName, password);

            var fixtureStore = _fixture
                .Build<StoreDto>()
                .With(s => s.FounderUserGuid, loginResult.UserGuid)
                .With(s => s.Description, "myStore")
                .Without(s => s.Rating)
                .Without(s => s.Guid)
                .Create();

            await storesService.CreateStoreAsync(fixtureStore);

            myStore = (await storesService.GetStoresAsync()).First();

            p1Dto = _fixture
                .Build<ProductDto>()
                .With(p => p.StoreGuid, myStore.Guid)
                .With(p => p.Amount, 20)
                .With(p => p.Category, "Toiletry")
                .With(p => p.Name, "Shampoo")
                .Without(p => p.Rating)
                .With(p => p.Price, 1)
               
                .Create();

            await storesService.CreateStoreProductAsync(p1Dto);

            p2Dto = _fixture
                .Build<ProductDto>()
                .With(p => p.StoreGuid, myStore.Guid)
                .With(p => p.Amount, 10)
                .With(p => p.Category, "Diary")
                .With(p => p.Name, "Milk from the moo")
                .Without(p => p.Rating)
                .With(p => p.Price, 1)
                .Create();

            await storesService.CreateStoreProductAsync(p2Dto);

            storesServiceWithData = new SecuredStoreService(storesService);
        }
        [Fact]
        public async Task RegisterAsync_ReturnsSuccessfulAuthenticationResult_WhenUsernameDoesNotExist()
        {
            // Arrange 
            var user = new UserDto { UserName = "guest" };
            const string password = "guestIsTheBest";

            // Act 
            var registerResult =  await authService.RegisterAsync(user, password);

            // Assert
            registerResult.Success.Should().BeTrue();
        }
        [Fact]
        public async Task RegisterAsync_ReturnsUnsuccessfulAuthenticationResult_WhenUsernameAlreadyExists()
        {
            // Arrange 
            var userGood = new UserDto { UserName = "guest" };
            const string passwordGood = "guestIsTheBest";
            var usernameDupBad = new UserDto {UserName = "guest"};
            const string passwordDupBad = "IAmTheBestGuest";

            // Act 
            var registerResultOk = await authService.RegisterAsync(userGood, passwordGood);
            var registerResultBad = await authService.RegisterAsync(usernameDupBad, passwordDupBad);

            // Assert
            registerResultOk.Success.Should().BeTrue();
            registerResultBad.Success.Should().BeFalse();
        }
        [Fact]
        public async Task LoginAsync_ReturnsSuccessfulAuthenticationResult_WhenUserIsAlreadyRegistered()
        {
            // Arrange 
            var user = new UserDto {UserName = "guest"};
            const string password = "guestIsTheBest";

            // Act
            await authService.RegisterAsync(user, password);
            var loginResult = await authService.LoginAsync(user.UserName, password);

            // Assert
            loginResult.Success.Should().BeTrue();
        }

        [Fact]
        public async Task LoginAsync_ReturnsUnsuccessfulAuthenticationResult_WhenUserHasNotRegistered()
        {
            // Arrange 
            var user = new UserDto {UserName = "guest"};
            const string password = "guestIsTheBest";

            // Act 
            await authService.RegisterAsync(user, password);
            var badPasswordLogin = await authService.LoginAsync(user.UserName, "another password");
            var badUsernameLogin = await authService.LoginAsync("guesty", password);

            // Assert
            badUsernameLogin.Success.Should().BeFalse();
            badPasswordLogin.Success.Should().BeFalse();
        }
        [Fact]
        public async Task GetStoreData__ReturnsCompatibleStoreData_WhenStoreExists()
        { 

            // Act 
            var storeData = await storesServiceWithData.GetStoresAsync();
            StoreDto myStore = storeData.First();
            

            // Assert
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

        [Fact]
        public async Task GetProductByKeywordAsync_ReturnsSuitableProducts_WhenKeyWordSuitsSomeProducts()
        {
            // Act 
            var toiletryProducts = await productsService.GetProductByKeywordAsync("Toiletry");
            // Assert
            toiletryProducts.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetProductByKeywordAsync_ReturnsEmptyList_WhenKeyWordDoesNotSuitAnyProducts()
        {
            // Act 
            var toiletryProducts = (await productsService.GetProductByKeywordAsync("ThisShouldNotWork!"));
            // Assert
            toiletryProducts.Should().BeEmpty();
        }


        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
