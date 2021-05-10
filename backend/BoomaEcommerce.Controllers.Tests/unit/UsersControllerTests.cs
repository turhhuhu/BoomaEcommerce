using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Api.Controllers;
using BoomaEcommerce.Services;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Stores;
using BoomaEcommerce.Services.Users;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BoomaEcommerce.Controllers.Tests.unit
{
    public class UsersControllerTests
    {
        private readonly UsersController _usersController;
        private readonly Mock<IUsersService> _usersServiceMock;
        private readonly Mock<IStoresService> _storeServicesMock;
        private readonly Guid _userGuidInClaims;
        public UsersControllerTests()
        {
            _storeServicesMock = new Mock<IStoresService>();
            _usersServiceMock = new Mock<IUsersService>();
            _usersController = new UsersController(_usersServiceMock.Object, _storeServicesMock.Object, Mock.Of<INotificationPublisher>());
            _userGuidInClaims = Guid.NewGuid();
            var fakeClaims = new List<Claim>
            {
                new("guid", _userGuidInClaims.ToString())
            };

            var fakeIdentity = new ClaimsIdentity(fakeClaims, "TestAuthType");
            var fakeClaimsPrincipal = new ClaimsPrincipal(fakeIdentity);
            _usersController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                User = fakeClaimsPrincipal,
                Request =
                {
                    Scheme = "https",
                    Host = new HostString("localhost", 5001)
                }
            };
        }

        [Fact]
        public async Task GetUserInfo_ShouldReturnOkResult_WhenUserExists()
        {
            // Arrange
            _usersServiceMock.Setup(x => x.GetUserInfoAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid guid) => new UserDto {Guid = guid});
            var expectedResult = new UserDto {Guid = _userGuidInClaims};

            // Act
            var userInfoResult = await _usersController.GetUserInfo();

            // Assert
            userInfoResult.Should().BeOfType<OkObjectResult>();
            var okResult = (OkObjectResult) userInfoResult;
            okResult.Value.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GetUserInfo_ShouldReturnNotFound_WhenUserInClaimsDoNotExist()
        {
            // Arrange
            _usersServiceMock.Setup(x => x.GetUserInfoAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid _) => null);

            // Act
            var userInfoResult = await _usersController.GetUserInfo();

            // Assert
            userInfoResult.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetCart_ShouldReturnCart_WhenUserLoggedInExists()
        {
            //Arrange
            _usersServiceMock.Setup(x => x.GetShoppingCartAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid guid) => new ShoppingCartDto {Guid = guid});
            var expectedResult = new ShoppingCartDto {Guid = _userGuidInClaims};

            // Act
            var cartResult = await _usersController.GetCart();

            // Assert
            cartResult.Should().BeOfType<OkObjectResult>();
            var okCartResult = (OkObjectResult) cartResult;
            okCartResult.Value.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GetCart_ShouldReturnStatusCodeInternalServerError_WhenCartIsNull()
        {
            //Arrange
            _usersServiceMock.Setup(x => x.GetShoppingCartAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid _) => null);

            // Act
            var cartResult = await _usersController.GetCart();

            // Assert
            cartResult.Should().BeOfType<StatusCodeResult>();
            var statusCodeResult = (StatusCodeResult)cartResult;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [Fact]
        public async Task CreateStore_ShouldReturnCreatedStore_WhenStoreServiceReturnsNotNullStore()
        {

            // Arrange
            var storeGuid =  Guid.NewGuid();
            _storeServicesMock.Setup(x => x.CreateStoreAsync(It.IsAny<StoreDto>()))
                .ReturnsAsync((StoreDto store) => store);
            var expectedResult = new StoreDto
            {
                Guid = storeGuid
            };

            // Act
            var storeResult = await _usersController.CreateStore(expectedResult);

            // Assert

            storeResult.Should().BeOfType<CreatedResult>();
            var createdStoreResult = (CreatedResult) storeResult;
            createdStoreResult.Location.Should().Be($"https://localhost:5001/stores/{storeGuid}");
            createdStoreResult.Value.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task CreateStore_ShouldReturnStatusCodeInternalServerError_WhenStoreServiceReturnsNullStore()
        {

            // Arrange
            var storeGuid = Guid.NewGuid();
            _storeServicesMock.Setup(x => x.CreateStoreAsync(It.IsAny<StoreDto>()))
                .ReturnsAsync((StoreDto _) => null);

            var store = new StoreDto
            {
                Guid = storeGuid
            };

            // Act
            var storeResult = await _usersController.CreateStore(store);

            // Assert

            storeResult.Should().BeOfType<StatusCodeResult>();
            var statusCodeResult = (StatusCodeResult)storeResult;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [Fact]
        public async Task CreateBasket_ShouldReturnCreatedBasket_WhenUserServiceCreatesBasket()
        {
            // Arrange

            var basketGuid = Guid.NewGuid();
            _usersServiceMock.Setup(x => x.CreateShoppingBasketAsync(It.IsAny<Guid>(), It.IsAny<ShoppingBasketDto>()))
                .ReturnsAsync((Guid _, ShoppingBasketDto shoppingBasket) => shoppingBasket);
            var shoppingBasketDto = new ShoppingBasketDto { Guid = basketGuid };

            // Act
            var basketResult = await _usersController.CreateBasket(shoppingBasketDto);

            // Assert
            basketResult.Should().BeOfType<CreatedAtActionResult>();
            var createdBasketResult = (CreatedAtActionResult) basketResult;
            createdBasketResult.Value.Should().BeEquivalentTo(shoppingBasketDto);
        }

        [Fact]
        public async Task CreateBasket_ShouldReturnStatusCodeInternalServerError_WhenUserServiceReturnsNull()
        {
            // Arrange
            var basketGuid = Guid.NewGuid();
            _usersServiceMock.Setup(x => x.CreateShoppingBasketAsync(It.IsAny<Guid>(), It.IsAny<ShoppingBasketDto>()))
                .ReturnsAsync((Guid _, ShoppingBasketDto _) => null);
            var shoppingBasketDto = new ShoppingBasketDto { Guid = basketGuid };

            // Act
            var basketResult = await _usersController.CreateBasket(shoppingBasketDto);

            // Assert
            basketResult.Should().BeOfType<StatusCodeResult>();
            var statusCodeResult = (StatusCodeResult)basketResult;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [Fact]
        public async Task DeletePurchaseProduct_ShouldReturnNoContent_WhenUserServiceReturnsTrue()
        {
            // Arrange
            _usersServiceMock.Setup(x =>
                    x.DeletePurchaseProductFromShoppingBasketAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync((Guid _, Guid _) => true);


            // Act
            var deletionResult = await _usersController.DeletePurchaseProduct(Guid.Empty, Guid.Empty);

            // Assert
            deletionResult.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeletePurchaseProduct_ShouldReturnNotFound_WhenUserServiceReturnsFalse()
        {
            // Arrange
            _usersServiceMock.Setup(x =>
                    x.DeletePurchaseProductFromShoppingBasketAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync((Guid _, Guid _) => false);


            // Act
            var deletionResult = await _usersController.DeletePurchaseProduct(Guid.Empty, Guid.Empty);

            // Assert
            deletionResult.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task CreatePurchaseProduct_ShouldReturnCreated_WhenUserServiceReturnsCreatedPurchaseProduct()
        {
            // Arrange
            var purchaseProductDto = new PurchaseProductDto { Guid = Guid.NewGuid() };
            _usersServiceMock.Setup(x =>
                    x.AddPurchaseProductToShoppingBasketAsync(It.IsAny<Guid>(), It.IsAny<PurchaseProductDto>()))
                .ReturnsAsync((Guid _, PurchaseProductDto pp) => pp);

            // Act
            var purchaseProductResult = await _usersController.CreatePurchaseProduct(Guid.Empty, purchaseProductDto);

            // Assert
            purchaseProductResult.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = (CreatedAtActionResult) purchaseProductResult;
            createdResult.Value.Should().BeEquivalentTo(purchaseProductDto);
        }

        [Fact]
        public async Task CreatePurchaseProduct_ShouldReturnNotFound_WhenUserServiceReturnsNull()
        {
            // Arrange
            var purchaseProductDto = new PurchaseProductDto { Guid = Guid.NewGuid() };
            _usersServiceMock.Setup(x =>
                    x.AddPurchaseProductToShoppingBasketAsync(It.IsAny<Guid>(), It.IsAny<PurchaseProductDto>()))
                .ReturnsAsync((Guid _, PurchaseProductDto _) => null);

            // Act
            var purchaseProductResult = await _usersController.CreatePurchaseProduct(Guid.Empty, purchaseProductDto);

            // Assert
            purchaseProductResult.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteBasket_ShouldReturnNoContent_WhenUserServiceReturnsTrue()
        {
            // Arrange
            _usersServiceMock.Setup(x => x.DeleteShoppingBasketAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid _) => true);

            // Act
            var deletionResult = await _usersController.DeleteBasket(Guid.Empty);

            // Assert
            deletionResult.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteBasket_ShouldReturnNotFound_WhenUserServiceReturnsFalse()
        {
            // Arrange
            _usersServiceMock.Setup(x => x.DeleteShoppingBasketAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid _) => false);

            // Act
            var deletionResult = await _usersController.DeleteBasket(Guid.Empty);

            // Assert
            deletionResult.Should().BeOfType<NotFoundResult>();
        }
    }
}
