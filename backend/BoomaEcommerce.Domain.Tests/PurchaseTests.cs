using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Xunit;

namespace BoomaEcommerce.Domain.Tests
{
    public class PurchaseTests
    {
        public PurchaseTests()
        {
            
        }
        
        [Fact]
        public async Task MakePurchase_ReturnsTrue_WhenStorePurchasesAreValid()
        {
            var sut = new Purchase {StorePurchases = TestData.GetTestValidStorePurchases()};

            var result = await sut.MakePurchase();

            result.Should().BeTrue();
        }
        
        [Fact]
        public async Task MakePurchase_ReturnsFalse_WhenStorePurchasesAreInvalid()
        {
            var sut = new Purchase {StorePurchases = TestData.GetTestInvalidStorePurchases()};

            var result = await sut.MakePurchase();

            result.Should().BeFalse();
        }

    }
}