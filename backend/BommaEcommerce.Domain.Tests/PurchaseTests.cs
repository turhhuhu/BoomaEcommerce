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
                .With(x => x.ProductLock, new SemaphoreSlim(30))
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

        private List<StorePurchase> GetTestValidStorePurchases()
        {
            var validStorePurchases = new List<StorePurchase>
            {
                _fixture.Build<StorePurchase>()
                    .With(x => x.ProductsPurchases, GetTestValidProductsPurchases())
                    .Create(),
                _fixture.Build<StorePurchase>()
                    .With(x => x.ProductsPurchases, GetTestValidProductsPurchases())
                    .Create(),
                _fixture.Build<StorePurchase>()
                    .With(x => x.ProductsPurchases, GetTestValidProductsPurchases())
                    .Create()
            };
            return validStorePurchases;
        }
        
        private List<StorePurchase> GetTestInvalidStorePurchases()
        {
            var validStorePurchases = new List<StorePurchase>
            {
                _fixture.Build<StorePurchase>()
                    .With(x => x.ProductsPurchases, GetTestInvalidProductsPurchases())
                    .Create(),
                _fixture.Build<StorePurchase>()
                    .With(x => x.ProductsPurchases, GetTestInvalidProductsPurchases())
                    .Create(),
                _fixture.Build<StorePurchase>()
                    .With(x => x.ProductsPurchases, GetTestInvalidProductsPurchases())
                    .Create()
            };
            return validStorePurchases;
        }

        [Fact]
        public async Task MakePurchase_ReturnsTrue_WhenStorePurchasesAreValid()
        {
            var sut = new Purchase {StorePurchases = GetTestValidStorePurchases()};

            var result = await sut.MakePurchase();

            result.Should().BeTrue();
        }
        
        [Fact]
        public async Task MakePurchase_ReturnsFalse_WhenStorePurchasesAreInvalid()
        {
            var sut = new Purchase {StorePurchases = GetTestInvalidStorePurchases()};

            var result = await sut.MakePurchase();

            result.Should().BeFalse();
        }

    }
}