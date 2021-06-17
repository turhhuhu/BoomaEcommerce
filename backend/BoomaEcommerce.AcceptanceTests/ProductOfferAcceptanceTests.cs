using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Domain.ProductOffer;
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
        private IStoresService _userStoresService;
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

            var storeService = provider.GetRequiredService<StoresService>();
            var authService = provider.GetRequiredService<IAuthenticationService>();
            
            var userService = provider.GetRequiredService<UsersService>();
            var storeOwnerAuthService = provider.GetRequiredService<IAuthenticationService>();
            await InitUser(authService, userService, storeService);
            await InitOwnerUser(storeService, storeOwnerAuthService);
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
            await InitUser(authService, userService, storeService);
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

        private async Task InitUser(IAuthenticationService authService, IUsersService userService, IStoresService storeService)
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

            var result = SecuredStoreService.CreateSecuredStoreService(loginResponse.Token,
                ServiceMockFactory.Secret, storeService, out _userStoresService);
            if (!result)
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
            var offers = await _userStoresService.GetAllUserProductOffers(UserGuid);
            //Assert
            offers.Count().Should().Be(1);
            offers.Should().Contain(po => po.Guid == productOffer.Guid);
        }

        [Fact]
        public async Task ApproveOffer_ShouldChangeOfferStateToApprove_WhenOnlyOwnerApprovesOffer()
        {
            await _storeService.ApproveOffer(_storeOwnership.Guid, productOffer.Guid);
            var offers = await _userStoresService.GetAllUserProductOffers(UserGuid);
            var offer = (offers.Where(offer => offer.Guid == productOffer.Guid)).FirstOrDefault();
            //Assert
            offer.Should().NotBeNull();
            offer.State.Should().Be(ProductOfferState.Approved);
        }

        [Fact]
        public async Task DeclineOffer_ShouldChangeOfferStateToDeclined_WhenANYOwnerDeclinesOffer()
        {
            await _storeService.DeclineOffer(_storeOwnership.Guid, productOffer.Guid);
            var offers = await _userStoresService.GetAllUserProductOffers(UserGuid);
            var offer = (offers.Where(offer => offer.Guid == productOffer.Guid)).FirstOrDefault();
            //Assert
            offer.Should().NotBeNull();
            offer.State.Should().Be(ProductOfferState.Declined);
        }

        [Fact]
        public async Task MakeCounterOffer_ShouldChangeOfferStateAndUpdateCounterOfferPrice_WhenANYOwnerSuggestsCounteOffer()
        {
            decimal cop = 8;
            await _storeService.MakeCounterOffer(_storeOwnership.Guid,8 ,productOffer.Guid);
            var offers = await _userStoresService.GetAllUserProductOffers(UserGuid);
            var offer = (offers.Where(offer => offer.Guid == productOffer.Guid)).FirstOrDefault();
            //Assert
            offer.Should().NotBeNull();
            offer.State.Should().Be(ProductOfferState.CounterOfferReceived);
            offer.CounterOfferPrice.Should().Be(cop);
        }
    }
}
