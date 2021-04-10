﻿using System;
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
            shoppingBasketDict[shoppingBasketGuid].Guid.Should().Be(shoppingBasketGuid);
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
        
        [Fact]
        public async Task AddPurchaseProductToShoppingBasketAsync_ReturnsTrueAndAddsPurchaseProduct_WhenShoppingBasketExistsAndPurchaseProductDtoIsValid()
        {
            // Arrange
            var purchaseProductDict = new Dictionary<Guid, PurchaseProduct>();
            var shoppingBasketDict = new Dictionary<Guid, ShoppingBasket>();
            var shoppingBasketGuid = Guid.NewGuid();
            var shoppingBasket= new ShoppingBasket{Guid = shoppingBasketGuid};
            shoppingBasketDict[shoppingBasketGuid] = shoppingBasket;
            var purchaseProductGuid = Guid.NewGuid();
            var purchaseProductDto = new PurchaseProductDto {Guid = purchaseProductGuid};
            var sut = GetUserService(shoppingBasketDict, null, purchaseProductDict);
            
            // Act
            var result = await sut.AddPurchaseProductToShoppingBasketAsync(shoppingBasketGuid, purchaseProductDto);

            // Assert
            result.Should().BeTrue();
            purchaseProductDict[purchaseProductGuid].Guid.Should().Be(purchaseProductGuid);
            shoppingBasket.PurchaseProducts.Contains(purchaseProductDict[purchaseProductGuid]).Should().BeTrue();
        }
        
        [Fact]
        public async Task AddPurchaseProductToShoppingBasketAsync_ReturnsFalse_WhenShoppingBasketDoesNotExists()
        {
            // Arrange
            var purchaseProductDict = new Dictionary<Guid, PurchaseProduct>();
            var shoppingBasketDict = new Dictionary<Guid, ShoppingBasket>();
            var purchaseProductGuid = Guid.NewGuid();
            var purchaseProductDto = new PurchaseProductDto {Guid = purchaseProductGuid};
            var sut = GetUserService(shoppingBasketDict, null, purchaseProductDict);
            
            // Act
            var result = await sut.AddPurchaseProductToShoppingBasketAsync(Guid.NewGuid(), purchaseProductDto);

            // Assert
            result.Should().BeFalse();
            purchaseProductDict.Keys.Should().NotContain(purchaseProductGuid);
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
            shoppingBasket.PurchaseProducts.Add(purchaseProduct);
            shoppingBasketDict[shoppingBasketGuid] = shoppingBasket;
            var sut = GetUserService(shoppingBasketDict, null, purchaseProductDict);
            
            // Act
            var result = await sut.DeletePurchaseProductFromShoppingBasketAsync(shoppingBasketGuid, purchaseProductGuid);

            // Assert
            result.Should().BeTrue();
            shoppingBasketDict[shoppingBasketGuid].PurchaseProducts.Should().NotContain(purchaseProduct);
            purchaseProductDict.Keys.Should().NotContain(purchaseProductGuid);
        }
        
        [Fact]
        public async Task DeletePurchaseProductFromShoppingBasketAsync_ReturnsFalse_WhenShoppingBasketExistsAndDoesNotHaveTheGivenPurchaseProduct()
        {
            // Arrange
            var purchaseProductDict = new Dictionary<Guid, PurchaseProduct>();
            var shoppingBasketDict = new Dictionary<Guid, ShoppingBasket>();
            var purchaseProductGuid = Guid.NewGuid();
            var purchaseProduct = new PurchaseProduct {Guid = purchaseProductGuid};
            purchaseProductDict[purchaseProductGuid] = purchaseProduct;
            var shoppingBasketGuid = Guid.NewGuid();
            var shoppingBasket = new ShoppingBasket {Guid = shoppingBasketGuid};
            shoppingBasketDict[shoppingBasketGuid] = shoppingBasket;
            var sut = GetUserService(shoppingBasketDict, null, purchaseProductDict);
            
            // Act
            var result = await sut.DeletePurchaseProductFromShoppingBasketAsync(shoppingBasketGuid, purchaseProductGuid);

            // Assert
            result.Should().BeFalse();
            shoppingBasketDict[shoppingBasketGuid].PurchaseProducts.Should().NotContain(purchaseProduct);
            purchaseProductDict.Keys.Should().Contain(purchaseProductGuid);
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