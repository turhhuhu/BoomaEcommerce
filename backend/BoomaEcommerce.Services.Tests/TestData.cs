using System;
using System.Collections.Generic;
using System.Threading;
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
            return new ()
            {
                Guid = guid,
                Amount = 10,
                Price = 10,
                IsSoftDeleted = false,
                ProductLock = new SemaphoreSlim(1),
                Name = "Test",
                Category = "TestCategory"
            };
        }
        
        public static Product GetTestProductFromStore(Guid productGuid, Guid storeGuid)
        {
            return new ()
            {
                Store = new Store{Guid = storeGuid},
                Guid = productGuid,
                Amount = 10,
                ProductLock = new SemaphoreSlim(1),
                Price = 10,
                IsSoftDeleted = false,
            };
        }


        public static List<PurchaseProductDto> GetTestValidPurchaseProductsDtos()
        {
            var validProductsPurchasesDtos = new List<PurchaseProductDto>
            {
                new()
                {
                    ProductDto = new ProductDto{Guid = Guid.NewGuid()},
                    Amount = 5,
                    Price = 50
                },
                new()
                {
                    ProductDto = new ProductDto{Guid = Guid.NewGuid()},
                    Amount = 5,
                    Price = 50
                },
                new()
                {
                    ProductDto = new ProductDto{Guid = Guid.NewGuid()},
                    Amount = 5,
                    Price = 50
                }
            };
            return validProductsPurchasesDtos;
        }
        
        private static List<PurchaseProductDto> GetTestInvalidPurchaseProductsDtos()
        {
            var invalidProductsPurchasesDtos = new List<PurchaseProductDto>
            {
                new()
                {
                    ProductDto = new ProductDto{Guid = Guid.NewGuid()},
                    Amount = 11,
                    Price = 50
                },
                new()
                {
                    ProductDto = new ProductDto{Guid = Guid.NewGuid()},
                    Amount = 11,
                    Price = 50
                },
                new()
                {
                    ProductDto = new ProductDto{Guid = Guid.NewGuid()},
                    Amount = 11,
                    Price = 50
                }
            };
            return invalidProductsPurchasesDtos;
        }

        public static List<StorePurchaseDto> GetTestValidStorePurchasesDtos()
        {
            var validProductsPurchasesDtos = new List<StorePurchaseDto>
            {
                new() {PurchaseProducts = GetTestValidPurchaseProductsDtos(), TotalPrice = 150},
                new() {PurchaseProducts = GetTestValidPurchaseProductsDtos(), TotalPrice = 150},
                new() {PurchaseProducts = GetTestValidPurchaseProductsDtos(), TotalPrice = 150}
            };
            return validProductsPurchasesDtos;
        }
        
        public static List<StorePurchaseDto> GetTestInvalidStorePurchasesDtos()
        {
            var validProductsPurchasesDtos = new List<StorePurchaseDto>
            {
                new() {PurchaseProducts = GetTestInvalidPurchaseProductsDtos()},
                new() {PurchaseProducts = GetTestInvalidPurchaseProductsDtos()},
                new() {PurchaseProducts = GetTestInvalidPurchaseProductsDtos()}
            };
            return validProductsPurchasesDtos;
        }
        
    }
}