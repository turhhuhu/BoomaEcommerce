using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Xunit;

namespace BoomaEcommerce.Domain.Tests
{
    public class StorePurchaseTests
    {
        public StorePurchaseTests()
        {

        }

        [Fact]
        public async Task MakePurchase_ReturnsTrue_WhenStorePurchasesAreValid()
        {
            var sut = new StorePurchase() {PurchaseProducts = TestData.GetTestValidProductsPurchases()};

            var result = await sut.PurchaseAllProducts();

            result.Should().BeTrue();
        }
        
        [Fact]
        public async Task MakePurchase_ReturnsFalse_WhenStorePurchasesAreInvalid()
        {
            var sut = new StorePurchase {PurchaseProducts = TestData.GetTestInvalidProductsPurchases()};

            var result = await sut.PurchaseAllProducts();

            result.Should().BeFalse();
        }
        
        [Fact]
        public void ValidatePrice_ReturnsTrue_WhenPriceValid()
        {
            var sut = new StorePurchase {PurchaseProducts = TestData.GetTestValidProductsPurchases(), TotalPrice = 150};

            var result = sut.ValidatePrice();

            result.Should().BeTrue();
        }
        
        [Fact]
        public void ValidatePrice_ReturnsFalse_WhenPriceInvalid()
        {
            var sut = new StorePurchase {PurchaseProducts = TestData.GetTestInvalidProductsPurchases(), TotalPrice = 150};

            var result = sut.ValidatePrice();

            result.Should().BeFalse();
        }
    }
}