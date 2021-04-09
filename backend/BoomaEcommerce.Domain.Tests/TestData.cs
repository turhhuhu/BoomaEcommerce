using System.Collections.Generic;
using System.Threading;
using AutoFixture;

namespace BoomaEcommerce.Domain.Tests
{
    public static class TestData
    {
        private static readonly IFixture Fixture = new Fixture();
        public static Product GetTestProduct()
        {
            return Fixture.Build<Product>()
                .With(x => x.Amount, 10)
                .With(x => x.Price, 10)
                .With(x => x.IsSoftDeleted, false)
                .With(x => x.ProductLock, new SemaphoreSlim(1))
                .Create();
        }
        
        public static List<PurchaseProduct> GetTestValidProductsPurchases()
        {
            var validProductsPurchases = new List<PurchaseProduct>
            {
                new(GetTestProduct(), 5),
                new(GetTestProduct(), 5),
                new(GetTestProduct(), 5)
            };

            return validProductsPurchases;
        }

        public static List<PurchaseProduct> GetTestInvalidProductsPurchases()
        {
            var invalidProductsPurchases = new List<PurchaseProduct>
            {
                new(GetTestProduct(), 15),
                new(GetTestProduct(), 5),
                new(GetTestProduct(), 0)
            };

            return invalidProductsPurchases;
        }

        public static List<StorePurchase> GetTestValidStorePurchases()
        {
            var validStorePurchases = new List<StorePurchase>
            {
                Fixture.Build<StorePurchase>()
                    .With(x => x.ProductsPurchases, GetTestValidProductsPurchases())
                    .Create(),
                Fixture.Build<StorePurchase>()
                    .With(x => x.ProductsPurchases, GetTestValidProductsPurchases())
                    .Create(),
                Fixture.Build<StorePurchase>()
                    .With(x => x.ProductsPurchases, GetTestValidProductsPurchases())
                    .Create()
            };
            return validStorePurchases;
        }
        
        public static List<StorePurchase> GetTestInvalidStorePurchases()
        {
            var validStorePurchases = new List<StorePurchase>
            {
                Fixture.Build<StorePurchase>()
                    .With(x => x.ProductsPurchases, GetTestInvalidProductsPurchases())
                    .Create(),
                Fixture.Build<StorePurchase>()
                    .With(x => x.ProductsPurchases, GetTestInvalidProductsPurchases())
                    .Create(),
                Fixture.Build<StorePurchase>()
                    .With(x => x.ProductsPurchases, GetTestInvalidProductsPurchases())
                    .Create()
            };
            return validStorePurchases;
        }
        
        
    }
}