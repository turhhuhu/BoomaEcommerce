using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using BoomaEcommerce.Domain;
using FluentAssertions;
using Xunit;

namespace BommaEcommerce.Domain.Tests
{
    public class PurchaseProductTests
    {
        private readonly IFixture _fixture;

        public PurchaseProductTests()
        {
            _fixture = new Fixture();
        }

        private Product GetTestProduct()
        {
            return _fixture.Build<Product>()
                .With(x => x.Amount, 10)
                .With(x => x.Price, 10)
                .With(x => x.IsSoftDeleted, false)
                .With(x => x.ProductLock, new SemaphoreSlim(1))
                .Create();
        }

        [Theory]
        [InlineData(1, 10)]
        [InlineData(5, 50)]
        [InlineData(10, 100)]
        public async Task CalculatePriceAsync_ReturnsCorrectPrice_WhenAmountIsValid(int amount, int expected)
        {
            var testProduct = GetTestProduct();
            var sut = new PurchaseProduct(testProduct, amount);

            var result = await sut.CalculatePriceAsync();

            result.Should().Be(expected);
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        public void Purchase_ReturnsTrue_WhenAmountIsValid(int amount)
        {
            var testProduct = GetTestProduct();
            var sut = new PurchaseProduct(testProduct, amount);

            var result = sut.Purchase();

            result.Should().BeTrue();
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        [InlineData(-10)]
        public void Purchase_ReturnsFalse_WhenAmountIsInvalid(int amount)
        {
            var testProduct = GetTestProduct();
            var sut = new PurchaseProduct(testProduct, amount);

            var result = sut.Purchase();

            result.Should().BeFalse();
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        public void ValidatePurchase_ReturnsTrue_WhenAmountIsValid(int amount)
        {
            var testProduct = GetTestProduct();
            var sut = new PurchaseProduct(testProduct, amount);

            var result = sut.ValidatePurchase();

            result.Should().BeTrue();
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        [InlineData(-10)]
        public void ValidatePurchase_ReturnsFalse_WhenAmountIsInvalid(int amount)
        {
            var testProduct = GetTestProduct();
            var sut = new PurchaseProduct(testProduct, amount);

            var result = sut.ValidatePurchase();

            result.Should().BeFalse();
        }

    }
}