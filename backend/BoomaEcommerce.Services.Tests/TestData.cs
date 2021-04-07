using System;
using System.Collections.Generic;
using AutoFixture;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;

namespace BoomaEcommerce.Services.Tests
{
    public static class TestData
    {
        private static readonly IFixture Fixture = new Fixture();
        public static Product GetTestProduct(Guid guid)
        {
            return Fixture.Build<Product>()
                .With(x => x.Guid, guid)
                .With(x => x.Amount, 10)
                .With(x => x.Price, 10)
                .With(x => x.IsSoftDeleted, false)
                .With(x => x.Name, "Test")
                .With(x => x.Category, "TestCategory")
                .Create();
        }
        
        public static Product GetTestProductFromStore(Guid productGuid, Guid storeGuid)
        {
            return Fixture.Build<Product>()
                .With(x => x.Store, new Store{Guid = storeGuid})
                .With(x => x.Guid, productGuid)
                .With(x => x.Amount, 10)
                .With(x => x.Price, 10)
                .With(x => x.IsSoftDeleted, false)
                .Create();
        }


        public static List<PurchaseProductDto> GetTestValidPurchaseProductsDtos()
        {
            var validProductsPurchasesDtos = new List<PurchaseProductDto>
            {
                new()
                {
                    ProductDto = new ProductDto{Guid = Guid.NewGuid()},
                    Amount = 5
                },
                new()
                {
                    ProductDto = new ProductDto{Guid = Guid.NewGuid()},
                    Amount = 5
                },
                new()
                {
                    ProductDto = new ProductDto{Guid = Guid.NewGuid()},
                    Amount = 5
                }
            };
            return validProductsPurchasesDtos;
        }

        public static List<StorePurchaseDto> GetTestValidStorePurchasesDtos()
        {
            var validProductsPurchasesDtos = new List<StorePurchaseDto>
            {
                new() {ProductsPurchases = GetTestValidPurchaseProductsDtos()},
                new() {ProductsPurchases = GetTestValidPurchaseProductsDtos()},
                new() {ProductsPurchases = GetTestValidPurchaseProductsDtos()}
            };
            return validProductsPurchasesDtos;
        }
    }
}