using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using BoomaEcommerce.Domain;
using FluentAssertions;
using Xunit;

namespace BommaEcommerce.Domain.Tests
{
    public class StorePurchaseTests
    {
        private readonly IFixture _fixture;
        public StorePurchaseTests()
        {
            _fixture = new Fixture();
        }
        
        private Product GetTestProduct()
        {
            return _fixture.Build<Product>()
                .With(x => x.Amount, 10)
                .With(x => x.Price, 10)
                .With(x => x.IsSoftDeleted, false)
                .Create();
        }

        private List<PurchaseProduct> GetTestValidProductsPurchases()
        {
            var validProductsPurchases = new List<PurchaseProduct>
            {
                new(GetTestProduct(), 5),
                new(GetTestProduct(), 5),
                new(GetTestProduct(), 5)
            };

            return validProductsPurchases;
        }
        
        private List<PurchaseProduct> GetTestInvalidProductsPurchases()
        {
            var invalidProductsPurchases = new List<PurchaseProduct>
            {
                new(GetTestProduct(), 15),
                new(GetTestProduct(), 5),
                new(GetTestProduct(), 0)
            };

            return invalidProductsPurchases;
        }

        [Fact]
        public async Task MakePurchase_ReturnsTrue_WhenStorePurchasesAreValid()
        {
            var sut = new StorePurchase() {ProductsPurchases = GetTestValidProductsPurchases()};

            var result = await sut.PurchaseProducts();

            result.Should().BeTrue();
        }
        
        [Fact]
        public async Task MakePurchase_ReturnsFalse_WhenStorePurchasesAreInvalid()
        {
            var sut = new StorePurchase {ProductsPurchases = GetTestInvalidProductsPurchases()};

            var result = await sut.PurchaseProducts();

            result.Should().BeFalse();
        }
    }
}