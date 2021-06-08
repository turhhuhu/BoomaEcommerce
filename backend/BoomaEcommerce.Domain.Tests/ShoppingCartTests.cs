using System;
using FluentAssertions;
using Xunit;

namespace BoomaEcommerce.Domain.Tests
{
    public class ShoppingCartTests
    {
        [Fact]
        public void AddShoppingBasket_ReturnsTrueAndAddsShoppingBasket_WhenShoppingBasketNotNull()
        {
            // Arrange
            var shoppingBasket = new ShoppingBasket{Store = new Store(null)};
            shoppingBasket.Store.Guid = Guid.NewGuid();
            var sut = new ShoppingCart(new User());
            
            // Act
            var result = sut.AddShoppingBasket(shoppingBasket);
            
            // Assert
            result.Should().BeTrue();
            sut.ShoppingBaskets.Contains(shoppingBasket).Should().BeTrue();
        }
        
        [Fact]
        public void AddShoppingBasket_ReturnsFalse_WhenShoppingBasketIsNull()
        {
            // Arrange
            var sut = new ShoppingCart(new User());
            
            // Act
            var result = sut.AddShoppingBasket(null);
            
            // Assert
            result.Should().BeFalse();
        }
        
        [Fact]
        public void RemoveShoppingBasket_ReturnsTrueAndRemovesShoppingBasket_WhenShoppingBasketExists()
        {
            var shoppingBasket = new ShoppingBasket{Store = new Store(null){Guid = Guid.NewGuid()}};
            var sut = new ShoppingCart(new User());
            sut.AddShoppingBasket(shoppingBasket);
            
            // Act
            var result = sut.RemoveShoppingBasket(shoppingBasket.Store.Guid);
            
            // Assert
            result.Should().BeTrue();
            sut.ShoppingBaskets.Contains(shoppingBasket).Should().BeFalse();
        }
        
        [Fact]
        public void RemoveShoppingBasket_ReturnsFalse_WhenPShoppingBasketDoesNotExists()
        {
            var shoppingBasket = new ShoppingBasket{Store = new Store(null){Guid = Guid.NewGuid()}};
            var sut = new ShoppingCart(new User());

            // Act
            var result = sut.RemoveShoppingBasket(shoppingBasket.Guid);
            
            // Assert
            result.Should().BeFalse();
            sut.ShoppingBaskets.Contains(shoppingBasket).Should().BeFalse();
        }
    }
}