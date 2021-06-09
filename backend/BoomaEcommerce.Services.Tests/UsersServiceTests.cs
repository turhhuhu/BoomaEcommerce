using System;
using System.Collections.Generic;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using AutoMapper;
using BoomaEcommerce.Data;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Users;
using BoomaEcommerce.Tests.CoreLib;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BoomaEcommerce.Services.Tests
{
    public class UsersServiceTests
    {
        private readonly IMapper _mapper = MapperFactory.GetMapper();
        private readonly Mock<ILogger<UsersService>> _logger = new();

        private UsersService GetUserService(
            IDictionary<Guid, ShoppingBasket> shoppingBaskets,
            IDictionary<Guid, ShoppingCart> shoppingCarts,
            IDictionary<Guid, User> users = null)
        {
            var userUnitOfWork = DalMockFactory.MockUserUnitOfWork(shoppingBaskets, shoppingCarts, users);
            return new UsersService(_mapper, _logger.Object, userUnitOfWork.Object);
        }

        [Fact]
        public async Task GetShoppingCartAsync_ReturnsShoppingCart_WhenShoppingCartExists()
        {
            // Arrange
            var shoppingCartsDict = new Dictionary<Guid, ShoppingCart>();
            var userGuid = Guid.NewGuid();
            var shoppingCart = new ShoppingCart(new User { Guid = userGuid });
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
            var users = new Dictionary<Guid, User>();
            var userGuid = Guid.NewGuid();
            users[userGuid] = (new User { Guid = userGuid });

            var sut = GetUserService(null, shoppingCartsDict, users);
            
            // Act
            var result = await sut.GetShoppingCartAsync(userGuid);

            // Assert
            result.Should().NotBeNull();
            result.Guid.Should().Be(userGuid);
        }
        
        [Fact]
        public async Task CreateShoppingBasketAsync_ReturnsTrueAndCreateShoppingBasket_WhenShoppingCartExistsAndShoppingCartDtoIsValid()
        {
            // Arrange
            var shoppingBasketDict = new Dictionary<Guid, ShoppingBasket>();
            var shoppingCartsDict = new Dictionary<Guid, ShoppingCart>();
            var userGuid = Guid.NewGuid();
            var shoppingCart = new ShoppingCart(new User {Guid = userGuid});
            shoppingCartsDict[shoppingCart.Guid] = shoppingCart;
            var shoppingBasketDto = 
                new ShoppingBasketDto{StoreGuid = Guid.NewGuid()};
            var sut = GetUserService(shoppingBasketDict, shoppingCartsDict, null);
            
            // Act
            var result = await sut.CreateShoppingBasketAsync(shoppingCart.Guid, shoppingBasketDto);

            // Assert
            result.Should().NotBeNull();
            shoppingCart.ShoppingBaskets.Contains(new ShoppingBasket {Store = new Store {Guid = shoppingBasketDto.StoreGuid}}).Should().BeTrue();
        }

        [Fact]
        public async Task AddPurchaseProductToShoppingBasketAsync_ReturnsTrueAndAddsPurchaseProduct_WhenShoppingBasketExistsAndPurchaseProductDtoIsValid()
        {
            // Arrange
            var shoppingBasketDict = new Dictionary<Guid, ShoppingBasket>();
            var user = new User
            {
                Guid = Guid.NewGuid()
            };
            var users = new Dictionary<Guid, User>();
            users[user.Guid] = user;

            var shoppingBasketGuid = Guid.NewGuid();
            var shoppingBasket= new ShoppingBasket{Guid = shoppingBasketGuid, Store = new Store()};

            shoppingBasketDict[shoppingBasketGuid] = shoppingBasket;
            var purchaseProductDto = new PurchaseProductDto();
            var sut = GetUserService(shoppingBasketDict, null, users);
            
            // Act
            var result = await sut.AddPurchaseProductToShoppingBasketAsync(user.Guid, shoppingBasketGuid, purchaseProductDto);

            // Assert
            result.Should().NotBeNull();
            shoppingBasket.PurchaseProducts.Should().NotBeEmpty();
        }
        
        [Fact]
        public async Task AddPurchaseProductToShoppingBasketAsync_ReturnsFalse_WhenShoppingBasketDoesNotExists()
        {
            // Arrange
            var shoppingBasketDict = new Dictionary<Guid, ShoppingBasket>();
            var purchaseProductDto = new PurchaseProductDto();
            var sut = GetUserService(shoppingBasketDict, null);
            
            // Act
            var result = await sut.AddPurchaseProductToShoppingBasketAsync(Guid.NewGuid(), Guid.NewGuid(), purchaseProductDto);

            // Assert
            result.Should().BeNull();
        }
        
        [Fact]
        public async Task DeletePurchaseProductFromShoppingBasketAsync_ReturnsTrueAndDeletedPurchaseProduct_WhenShoppingBasketExistsAndHasTheGivenPurchaseProduct()
        {
            // Arrange
            var purchaseProductDict = new Dictionary<Guid, PurchaseProduct>();
            var shoppingBasketDict = new Dictionary<Guid, ShoppingBasket>();
            var purchaseProductGuid = Guid.NewGuid();
            var purchaseProduct = new PurchaseProduct {Guid = purchaseProductGuid};
            purchaseProductDict[purchaseProductGuid] = purchaseProduct;
            var shoppingBasketGuid = Guid.NewGuid();
            var shoppingBasket = new ShoppingBasket {Guid = shoppingBasketGuid};
            shoppingBasket.AddPurchaseProduct(purchaseProduct);
            shoppingBasketDict[shoppingBasketGuid] = shoppingBasket;
            var sut = GetUserService(shoppingBasketDict, null);
            
            // Act
            var result = await sut.DeletePurchaseProductFromShoppingBasketAsync(shoppingBasketGuid, purchaseProductGuid);

            // Assert
            result.Should().BeTrue();
            shoppingBasketDict[shoppingBasketGuid].PurchaseProducts.Should().NotContain(x => x.Guid == purchaseProductGuid);
        }
        
        [Fact]
        public async Task DeletePurchaseProductFromShoppingBasketAsync_ReturnsFalse_WhenShoppingBasketExistsAndDoesNotHaveTheGivenPurchaseProduct()
        {
            // Arrange
            var shoppingBasketDict = new Dictionary<Guid, ShoppingBasket>();
            var purchaseProductGuid = Guid.NewGuid();
            var purchaseProduct = new PurchaseProduct {Guid = purchaseProductGuid};
            var shoppingBasketGuid = Guid.NewGuid();
            var shoppingBasket = new ShoppingBasket {Guid = shoppingBasketGuid};
            shoppingBasketDict[shoppingBasketGuid] = shoppingBasket;
            var sut = GetUserService(shoppingBasketDict, null);
            
            // Act
            var result = await sut.DeletePurchaseProductFromShoppingBasketAsync(shoppingBasketGuid, purchaseProductGuid);

            // Assert
            result.Should().BeFalse();
            shoppingBasketDict[shoppingBasketGuid].PurchaseProducts.Should().NotContain(x => x.Guid == purchaseProductGuid);
        }
        
        [Fact]
        public async Task DeletePurchaseProductFromShoppingBasketAsync_ReturnsFalse_WhenShoppingBasketDoesNotExist()
        {
            // Arrange
            var shoppingBasketDict = new Dictionary<Guid, ShoppingBasket>();
            var shoppingBasketGuid = Guid.NewGuid();
            var sut = GetUserService(shoppingBasketDict, null, null);
            
            // Act
            var result = await sut.DeletePurchaseProductFromShoppingBasketAsync(shoppingBasketGuid, Guid.NewGuid());

            // Assert
            result.Should().BeFalse();
        }
        
        [Fact]
        public async Task DeleteShoppingBasketAsync_ReturnsTrueAndDeletesBasket_WhenShoppingBasketExists()
        {
            // Arrange
            var shoppingBasketDict = new Dictionary<Guid, ShoppingBasket>();
            var shoppingBasketGuid = Guid.NewGuid();
            var shoppingBasket = new ShoppingBasket {Guid = shoppingBasketGuid};
            shoppingBasketDict[shoppingBasketGuid] = shoppingBasket;
            var sut = GetUserService(shoppingBasketDict, null, null);
            
            // Act
            var result = await sut.DeleteShoppingBasketAsync(shoppingBasketGuid);

            // Assert
            result.Should().BeTrue();
            shoppingBasketDict.Keys.Should().NotContain(shoppingBasketGuid);
        }
        
        [Fact]
        public async Task DeleteShoppingBasketAsync_ReturnsTrue_WhenShoppingBasketDoesNotExists()
        {
            // Arrange
            var shoppingBasketDict = new Dictionary<Guid, ShoppingBasket>();
            var sut = GetUserService(shoppingBasketDict, null, null);
            
            // Act
            var result = await sut.DeleteShoppingBasketAsync(Guid.NewGuid());

            // Assert
            result.Should().BeTrue();
        }
        
    }
}