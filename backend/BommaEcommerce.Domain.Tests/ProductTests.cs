using BoomaEcommerce.Domain;
using FluentAssertions;
using Xunit;

namespace BommaEcommerce.Domain.Tests
{
    public class ProductTests
    {
        [Theory]
        [InlineData(3, 1, 3)]
        [InlineData(2.2, 2, 4.4)]
        [InlineData(5.5, 0, 0)]
        public void CalculatePrice_MultiplyPriceAndAmount(double price, int amount, double expected)
        {
            var sut = new Product {Price = price};

            var result = sut.CalculatePrice(amount);

            result.Should().Be(expected);
        }
        [Theory]
        [InlineData(10, 5)]
        [InlineData(2, 1)]
        [InlineData(5, 5)]
        public void ValidateAmount_ReturnTrue_WhenAmountIsValid(int initialAmount, int amount)
        {
            var sut = new Product {Amount = initialAmount};

            var result = sut.ValidateAmount(amount);

            result.Should().BeTrue();
        }
        
        [Theory]
        [InlineData(5, 10)]
        [InlineData(0, 0)]
        [InlineData(-10, -5)]
        public void ValidateAmount_ReturnFalse_WhenAmountIsInvalid(int initialAmount, int amount)
        {
            var sut = new Product {Amount = initialAmount};

            var result = sut.ValidateAmount(amount);

            result.Should().BeFalse();
        }

        [Theory]
        [InlineData(10, 5, 5)]
        [InlineData(2, 1, 1)]
        [InlineData(5, 5, 0)]
        public void PurchaseAmount_ReturnsTrueAndDecreaseAmount_WhenAmountIsValid(int initialAmount,
            int amount, int expectedAmountAfterDecrease)
        {
            var sut = new Product() {Amount = initialAmount};

            var result = sut.PurchaseAmount(amount);

            result.Should().BeTrue();
            sut.Amount.Should().Be(expectedAmountAfterDecrease);
        }
        
        [Theory]
        [InlineData(5, 10, 5)]
        [InlineData(0, 0, 0)]
        [InlineData(-10, -5, -10)]
        public void PurchaseAmount_ReturnsFalseAndDoesNotDecreaseAmount_WhenAmountIsInvalid(int initialAmount,
            int amount, int expectedAmountAfterDecrease)
        {
            var sut = new Product() {Amount = initialAmount};

            var result = sut.PurchaseAmount(amount);

            result.Should().BeFalse();
            sut.Amount.Should().Be(expectedAmountAfterDecrease);
        }
        
    }
}