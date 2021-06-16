using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.Authentication;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.DTO.ProductOffer;
using BoomaEcommerce.Services.Purchases;
using BoomaEcommerce.Services.Stores;
using BoomaEcommerce.Services.Users;
using BoomaEcommerce.Tests.CoreLib;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BoomaEcommerce.AcceptanceTests
{
    public class ProductOfferAcceptanceTests : TestsBase
    {
        private IStoresService _storeService;
        private IUsersService _usersService;
        
        private IAuthenticationService _storeOwnerAuthService;
        private IFixture _fixture;
        private IAuthenticationService _authenticationService;
        private Guid UserGuid;
        private StoreOwnershipDto _storeOwnership;
        private ProductDto product;
        private ProductOfferDto productOffer; 



        public ProductOfferAcceptanceTests(SharedDatabaseFixture dataBaseFixture) : base(dataBaseFixture)
        {

        }

        public override async Task InitEfCoreDb(ServiceProvider provider)
        {
            _fixture = new Fixture();
            var serviceMockFactory = new ServiceMockFactory();
            var storeService = serviceMockFactory.MockStoreService();
            var userService = serviceMockFactory.MockUserService();
            var authService = serviceMockFactory.MockAuthenticationService();
            var storeOwnerAuthService = serviceMockFactory.MockAuthenticationService();
            await InitUser(authService,userService);
            await InitOwnerUser(storeService,storeOwnerAuthService);
            await InitOffer();
        }

        public override async Task InitInMemoryDb()
        {
            _fixture = new Fixture();
            var serviceMockFactory = new ServiceMockFactory();
            var storeService = serviceMockFactory.MockStoreService();
            var userService = serviceMockFactory.MockUserService();
            var authService = serviceMockFactory.MockAuthenticationService();
            var storeOwnerAuthService = serviceMockFactory.MockAuthenticationService();
            await InitUser(authService, userService);
            await InitOwnerUser(storeService, storeOwnerAuthService);
            await InitOffer();
        }

        private async Task InitOffer()
        {
            var offerDto = _fixture.
                Build<ProductOfferDto>()
                .With(o => o.Product, product)
                .With(o => o.OfferPrice, 5)
                .With(o => o.State, ProductOfferStateDto.Pending)
                .With(o => o.UserGuid, UserGuid)
                .Without(o => o.Guid)
                .Without(o => o.CounterOfferPrice)
                .Create();
            productOffer = await _usersService.CreateProductOffer(offerDto);
        }

        private async Task InitOwnerUser(IStoresService storeService, IAuthenticationService authService)
        {
            var user = new UserDto { UserName = "Omik" };
            const string password = "Omik1337";

            await authService.RegisterAsync(user, password);
            var loginResponse = await authService.LoginAsync(user.UserName, password);

            var fixtureStore = _fixture
                .Build<StoreDto>()
                .With(s => s.FounderUserGuid, loginResponse.UserGuid)
                .Without(s => s.Rating)
                .Without(s => s.Guid)
                .Create();
          
            var store = await storeService.CreateStoreAsync(fixtureStore);

            var productDto = _fixture
               .Build<ProductDto>()
               .With(p1 => p1.Price, 10)
               .With(p1 => p1.Amount, 10)
               .With(p1 => p1.StoreGuid, store.Guid)
               .Without(p1 => p1.Guid)
               .Without(p1 => p1.Rating).Create();

            product = await storeService.CreateStoreProductAsync(productDto);
            
                _storeOwnership = (await storeService.GetAllStoreOwnerShipsAsync(loginResponse.UserGuid)).First();
            var result = SecuredStoreService.CreateSecuredStoreService(loginResponse.Token,
                ServiceMockFactory.Secret, storeService, out _storeService);
            if (!result)
            {
                throw new Exception("This shouldn't happen");

            }
        }

        private async Task InitUser(IAuthenticationService authService, IUsersService userService)
        {
            var user = new UserDto { UserName = "Benjaminio" };
            const string password = "OMG";

            await authService.RegisterAsync(user, password);
            var loginResponse = await authService.LoginAsync(user.UserName, password);

            UserGuid = loginResponse.UserGuid;


            var createUserServiceResult = SecuredUserService.CreateSecuredUserService(loginResponse.Token,
             ServiceMockFactory.Secret, userService, out _usersService);
            if (!createUserServiceResult)
            {
                throw new Exception("This shouldn't happen");
            }

        }


        [Fact]
        public async Task GetAllOwnerProductOffers_ShouldReturnOwnerOffers()
        {
            //Arrange

            //Act
            var offers = await _storeService.GetAllOwnerProductOffers(_storeOwnership.Guid);
            //Assert
            offers.Count().Should().Be(1);
            offers.Should().Contain(po => po.Guid == productOffer.Guid);
        }

        [Fact]
        public async Task GetAllUserProductOffers_ShouldReturnUserOffers()
        {
            //Arrange

            //Act
            var offers = await _storeService.GetAllUserProductOffers(UserGuid);
            //Assert
            offers.Count().Should().Be(1);
            offers.Should().Contain(po => po.Guid == productOffer.Guid);
        }


        [Fact]
        public async Task GetProductOffer_ShouldReturnProductOffer()
        {
            //Arrange

            //Act
            var offers = await _storeService.GetAllUserProductOffers(UserGuid);
            //Assert
            offers.Count().Should().Be(1);
            offers.Should().Contain(po => po.Guid == productOffer.Guid);
        }

        // public async Task<ProductOfferDto> GetProductOffer(Guid offerGuid)
        //public async Task<ProductOfferDto> MakeCounterOffer(Guid ownerGuid, decimal counterOfferPrice, Guid offerGuid)
        //public async Task DeclineOffer(Guid ownerGuid, Guid productOfferGuid)
        //public async Task ApproveOffer(Guid ownerGuid, Guid productOfferGuid)




    }
}
