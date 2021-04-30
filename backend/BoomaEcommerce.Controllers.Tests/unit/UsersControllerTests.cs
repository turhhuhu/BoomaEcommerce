using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Api.Controllers;
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
            _usersController = new UsersController(_usersServiceMock.Object, _storeServicesMock.Object);
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

    }
}
