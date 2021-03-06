using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Xunit;

namespace BoomaEcommerce.Domain.Tests
{
    public class PurchaseProductTests
    {

        public PurchaseProductTests()
        {
            
        }

        [Theory]
        [InlineData(1, 10)]
        [InlineData(5, 50)]
        [InlineData(10, 100)]
        public async Task CalculatePriceAsync_ReturnsCorrectPrice_WhenAmountIsValid(int amount, int expected)
        {
            var testProduct = TestData.GetTestProduct();
            var sut = new PurchaseProduct(testProduct, amount, expected);

            var result = await sut.CalculatePriceAsync();

            result.Should().Be(expected);
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        public void Purchase_ReturnsTrue_WhenAmountIsValid(int amount)
        {
            var testProduct = TestData.GetTestProduct();
            var sut = new PurchaseProduct(testProduct, amount, amount);

            var result = sut.Purchase();

            result.Should().BeTrue();
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        [InlineData(-10)]
        public void Purchase_ReturnsFalse_WhenAmountIsInvalid(int amount)
        {
            var testProduct = TestData.GetTestProduct();
            var sut = new PurchaseProduct(testProduct, amount, amount);

            var result = sut.Purchase();

            result.Should().BeFalse();
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        public void ValidatePurchase_ReturnsTrue_WhenAmountIsValid(int amount)
        {
            var testProduct = TestData.GetTestProduct();
            var sut = new PurchaseProduct(testProduct, amount, amount);

            var result = sut.ValidatePurchase();

            result.Should().BeTrue();
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        [InlineData(-10)]
        public void ValidatePurchase_ReturnsFalse_WhenAmountIsInvalid(int amount)
        {
            var testProduct = TestData.GetTestProduct();
            var sut = new PurchaseProduct(testProduct, amount, amount);

            var result = sut.ValidatePurchase();

            result.Should().BeFalse();
        }
        
        [Theory]
        [InlineData(1, 10)]
        [InlineData(5, 50)]
        [InlineData(10, 100)]
        public void ValidatePrice_ReturnsTrue_WhenPriceValid(int amount, int expected)
        {
            var testProduct = TestData.GetTestProduct();
            var sut = new PurchaseProduct(testProduct, amount, expected);

            var result = sut.ValidatePrice(new List<ProductOffer.ProductOffer>());

            result.Should().BeTrue();
        }
        
        [Theory]
        [InlineData(1, 5)]
        [InlineData(5, 10)]
        [InlineData(10, 15)]
        public void ValidatePrice_ReturnsTrue_WhenPriceIsInvalid(int amount, int expected)
        {
            var testProduct = TestData.GetTestProduct();
            var sut = new PurchaseProduct(testProduct, amount, expected);

            var result = sut.ValidatePrice(new List<ProductOffer.ProductOffer>());

            result.Should().BeFalse();
        }

    }
}