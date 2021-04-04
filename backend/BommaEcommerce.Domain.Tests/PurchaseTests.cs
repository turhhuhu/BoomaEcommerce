using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using BoomaEcommerce.Domain;
using FluentAssertions;
using Xunit;

namespace BommaEcommerce.Domain.Tests
{
    public class PurchaseTests
    {
        private readonly IFixture _fixture;
        public PurchaseTests()
        {
            _fixture = new Fixture();
        }
        
        private Product GetTestProduct()
        {
            return _fixture.Build<Product>()
                .With(x => x.Amount, 10)
                .With(x => x.Price, 10)
                .With(x => x.ProductLock, new SemaphoreSlim(3))
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
        public async Task PurchaseProducts_ReturnsTrue_WhenProductsPurchasesAreValid()
        {
            var sut = new Purchase {ProductsPurchases = GetTestValidProductsPurchases()};

            var result = await sut.MakePurchase();

            result.Should().BeTrue();
        }
        
        [Fact]
        public async Task PurchaseProducts_ReturnsFalse_WhenProductsPurchasesAreInvalid()
        {
            var sut = new Purchase {ProductsPurchases = GetTestInvalidProductsPurchases()};

            var result = await sut.MakePurchase();

            result.Should().BeFalse();
        }

    }
}