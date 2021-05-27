using System;
using FluentAssertions;
using Xunit;

namespace BoomaEcommerce.Domain.Tests
{
    public class ShoppingBasketTests
    {
        [Fact]
        public void AddPurchaseProduct_ReturnsTrueAndAddsPurchaseProduct_WhenPurchaseProductNotNull()
        {
            // Arrange
            var purchaseProduct = new PurchaseProduct();
            var sut = new ShoppingBasket();
            
            // Act
            var result = sut.AddPurchaseProduct(purchaseProduct);
            
            // Assert
            result.Should().BeTrue();
            sut.PurchaseProducts.ContainsKey(purchaseProduct.Guid).Should().BeTrue();
        }
        
        [Fact]
        public void AddPurchaseProduct_ReturnsFalse_WhenPurchaseProductIsNull()
        {
            // Arrange
            var sut = new ShoppingBasket();
            
            // Act
            var result = sut.AddPurchaseProduct(null);
            
            // Assert
            result.Should().BeFalse();
        }
        
        [Fact]
        public void RemovePurchaseProduct_ReturnsTrueAndRemovesPurchaseProduct_WhenPurchaseProductExists()
        {
            // Arrange
            var purchaseProduct = new PurchaseProduct();
            var sut = new ShoppingBasket();
            sut.PurchaseProducts.Add(purchaseProduct.Guid, purchaseProduct);
            
            // Act
            var result = sut.RemovePurchaseProduct(purchaseProduct.Guid);
            
            // Assert
            result.Should().BeTrue();
            sut.PurchaseProducts.ContainsKey(purchaseProduct.Guid).Should().BeFalse();
        }
        
        [Fact]
        public void RemovePurchaseProduct_ReturnsFalse_WhenPurchaseProductDoesNotExists()
        {
            // Arrange
            var purchaseProduct = new PurchaseProduct();
            var sut = new ShoppingBasket();

            // Act
            var result = sut.RemovePurchaseProduct(purchaseProduct.Guid);
            
            // Assert
            result.Should().BeFalse();
            sut.PurchaseProducts.ContainsKey(purchaseProduct.Guid).Should().BeFalse();
        }
    }
}