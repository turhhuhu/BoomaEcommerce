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
            var sut = new StorePurchase() {ProductsPurchases = TestData.GetTestValidProductsPurchases()};

            var result = await sut.PurchaseProducts();

            result.Should().BeTrue();
        }
        
        [Fact]
        public async Task MakePurchase_ReturnsFalse_WhenStorePurchasesAreInvalid()
        {
            var sut = new StorePurchase {ProductsPurchases = TestData.GetTestInvalidProductsPurchases()};

            var result = await sut.PurchaseProducts();

            result.Should().BeFalse();
        }
    }
}