using System.Collections.Generic;
using System.Threading;
using AutoFixture;
using BoomaEcommerce.Domain.Discounts;
using BoomaEcommerce.Domain.Policies;

namespace BoomaEcommerce.Domain.Tests
{
    public static class TestData
    {
        private static readonly IFixture Fixture = new Fixture();
        public static Product GetTestProduct()
        {
            return new ()
            {
                Amount = 10,
                Price = 10,
                IsSoftDeleted = false,
                ProductLock = new SemaphoreSlim(1)
            };
        }
        
        public static List<PurchaseProduct> GetTestValidProductsPurchases()
        {
            var validProductsPurchases = new List<PurchaseProduct>
            {
                new(GetTestProduct(), 5, 50, 50),
                new(GetTestProduct(), 5, 50, 50),
                new(GetTestProduct(), 5, 50, 50)
            };

            return validProductsPurchases;
        }

        public static List<PurchaseProduct> GetTestInvalidProductsPurchases()
        {
            var invalidProductsPurchases = new List<PurchaseProduct>
            {
                new(GetTestProduct(), 15, 150),
                new(GetTestProduct(), 5, 50),
                new(GetTestProduct(), 0, 0)
            };

            return invalidProductsPurchases;
        }

        public static List<StorePurchase> GetTestValidStorePurchases()
        {
            var validStorePurchases = new List<StorePurchase>
            {
                new ()
                {
                    Store = new Store(new User(), Policy.Empty, Discount.Empty),
                    PurchaseProducts = GetTestValidProductsPurchases(),
                    DiscountedPrice = 150,
                    TotalPrice = 150
                },
                new ()
                {
                    Store = new Store(new User(), Policy.Empty, Discount.Empty),
                    PurchaseProducts = GetTestValidProductsPurchases(),
                    DiscountedPrice = 150,
                    TotalPrice = 150
                },
                new ()
                {
                    Store = new Store(new User(), Policy.Empty, Discount.Empty),
                    PurchaseProducts = GetTestValidProductsPurchases(),
                    DiscountedPrice = 150,
                    TotalPrice = 150
                }
            };
            return validStorePurchases;
        }
        
        public static List<StorePurchase> GetTestInvalidStorePurchases()
        {
            var validStorePurchases = new List<StorePurchase>
            {
                new ()
                {
                    PurchaseProducts = GetTestInvalidProductsPurchases()
                },
                new ()
                {
                    PurchaseProducts = GetTestInvalidProductsPurchases()
                },
                new ()
                {
                    PurchaseProducts = GetTestInvalidProductsPurchases()
                }
            };
            return validStorePurchases;
        }
        
        
    }
}