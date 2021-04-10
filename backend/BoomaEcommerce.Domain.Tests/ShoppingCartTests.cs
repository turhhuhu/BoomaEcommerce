using FluentAssertions;
using Xunit;

namespace BoomaEcommerce.Domain.Tests
{
    public class ShoppingCartTests
    {
        [Fact]
        public void AddShoppingBasket_ReturnsTrueAndAddsPurchaseProduct_WhenPurchaseProductNotNull()
        {
            // Arrange
            var shoppingBasket = new ShoppingBasket();
            var sut = new ShoppingCart();
            
            // Act
            var result = sut.AddShoppingBasket(shoppingBasket);
            
            // Assert
            result.Should().BeTrue();
            sut.Baskets.Contains(shoppingBasket).Should().BeTrue();
        }
        
        [Fact]
        public void AddShoppingBasket_ReturnsFalse_WhenPurchaseProductIsNull()
        {
            // Arrange
            var sut = new ShoppingCart();
            
            // Act
            var result = sut.AddShoppingBasket(null);
            
            // Assert
            result.Should().BeFalse();
        }
        
        [Fact]
        public void RemoveShoppingBasket_ReturnsTrueAndRemovesPurchaseProduct_WhenPurchaseProductExists()
        {
            var shoppingBasket = new ShoppingBasket();
            var sut = new ShoppingCart();
            sut.Baskets.Add(shoppingBasket);
            
            // Act
            var result = sut.RemoveShoppingBasket(shoppingBasket.Guid);
            
            // Assert
            result.Should().BeTrue();
            sut.Baskets.Contains(shoppingBasket).Should().BeFalse();
        }
        
        [Fact]
        public void RemoveShoppingBasket_ReturnsFalse_WhenPurchaseProductDoesNotExists()
        {
            var shoppingBasket = new ShoppingBasket();
            var sut = new ShoppingCart();

            // Act
            var result = sut.RemoveShoppingBasket(shoppingBasket.Guid);
            
            // Assert
            result.Should().BeFalse();
            sut.Baskets.Contains(shoppingBasket).Should().BeFalse();
        }
    }
}