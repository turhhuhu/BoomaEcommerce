using System;
using System.Collections.Generic;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using AutoMapper;
using BoomaEcommerce.Data;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Users;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BoomaEcommerce.Services.Tests
{
    public class UsersServiceTests
    {
        private readonly IMapper _mapper = MapperFactory.GetMapper();
        private readonly Mock<ILogger<UsersService>> _logger = new();

        private UsersService GetUserService(IDictionary<Guid, ShoppingBasket> shoppingBaskets,
            IDictionary<Guid, ShoppingCart> shoppingCarts, IDictionary<Guid, PurchaseProduct> purchaseProducts
            )
        {
            var shoppingBasketRepoMock = DalMockFactory.MockRepository(shoppingBaskets);
            var shoppingCartRepoMock = DalMockFactory.MockRepository(shoppingCarts);
            var purchaseProductRepoMock = DalMockFactory.MockRepository(purchaseProducts);

            var userUnitOfWork = new Mock<IUserUnitOfWork>();
            userUnitOfWork.SetupGet(x => x.ShoppingBasketRepo).Returns(shoppingBasketRepoMock?.Object);
            userUnitOfWork.SetupGet(x => x.ShoppingCartRepo).Returns(shoppingCartRepoMock?.Object);
            userUnitOfWork.SetupGet(x => x.PurchaseProductRepo).Returns(purchaseProductRepoMock?.Object);
            
            return new UsersService(_mapper, _logger.Object, userUnitOfWork.Object);
        }

        [Fact]
        public async Task GetShoppingCartAsync_ReturnsShoppingCart_WhenShoppingCartExists()
        {
            // Arrange
            var shoppingCartsDict = new Dictionary<Guid, ShoppingCart>();
            var userGuid = Guid.NewGuid();
            var shoppingCart = new ShoppingCart {User = new User {Guid = userGuid}};
            shoppingCartsDict[shoppingCart.Guid] = shoppingCart;
            var sut = GetUserService(null, shoppingCartsDict, null);
            
            // Act
            var result = await sut.GetShoppingCartAsync(userGuid);

            // Assert
            result.Guid.Should().Be(shoppingCart.Guid);
        }
        
        [Fact]
        public async Task GetShoppingCartAsync_ReturnsNewShoppingCart_WhenShoppingCartDoesNotExists()
        {
            // Arrange
            var shoppingCartsDict = new Dictionary<Guid, ShoppingCart>();
            var userGuid = Guid.NewGuid();
            var sut = GetUserService(null, shoppingCartsDict, null);
            
            // Act
            var result = await sut.GetShoppingCartAsync(userGuid);

            // Assert
            result.User.Guid.Should().Be(userGuid);
        }
        
        [Fact]
        public async Task CreateShoppingBasketAsync_ReturnsTrueAndCreateShoppingBasket_WhenShoppingCartExistsAndShoppingCartDtoIsValid()
        {
            // Arrange
            var shoppingBasketDict = new Dictionary<Guid, ShoppingBasket>();
            var shoppingCartsDict = new Dictionary<Guid, ShoppingCart>();
            var userGuid = Guid.NewGuid();
            var shoppingCart = new ShoppingCart {User = new User {Guid = userGuid}};
            shoppingCartsDict[shoppingCart.Guid] = shoppingCart;
            var shoppingBasketGuid = Guid.NewGuid();
            var shoppingBasketDto = new ShoppingBasketDto{Guid = shoppingBasketGuid};
            var sut = GetUserService(shoppingBasketDict, shoppingCartsDict, null);
            
            // Act
            var result = await sut.CreateShoppingBasketAsync(shoppingCart.Guid, shoppingBasketDto);

            // Assert
            result.Should().BeTrue();
            shoppingBasketDict[shoppingBasketGuid].Should().NotBeNull();
            shoppingCart.Baskets.Contains(shoppingBasketDict[shoppingBasketGuid]).Should().BeTrue();
        }
        [Fact]
        public async Task CreateShoppingBasketAsync_ReturnsFalse_WhenShoppingCartDoesNotExists()
        {
            // Arrange
            var shoppingBasketDict = new Dictionary<Guid, ShoppingBasket>();
            var shoppingCartsDict = new Dictionary<Guid, ShoppingCart>();
            var shoppingBasketGuid = Guid.NewGuid();
            var shoppingBasketDto = new ShoppingBasketDto{Guid = shoppingBasketGuid};
            var sut = GetUserService(shoppingBasketDict, shoppingCartsDict, null);
            
            // Act
            var result = await sut.CreateShoppingBasketAsync(Guid.NewGuid(), shoppingBasketDto);

            // Assert
            result.Should().BeFalse();
            shoppingBasketDict.Keys.Should().NotContain(shoppingBasketGuid);
        }

    }
}