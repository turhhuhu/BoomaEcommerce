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
        public static Product GetTestProduct(Guid guid, Guid storeGuid = default)
        {
            return new ()
            {
                Guid = guid,
                Amount = 10,
                Price = 10,
                IsSoftDeleted = false,
                ProductLock = new SemaphoreSlim(1),
                Name = "Test",
                Category = "TestCategory",
                Store = new Store(null) { Guid = storeGuid}
            };
        }
        
        public static Product GetTestProductFromStore(Guid productGuid, Guid storeGuid)
        {
            return new ()
            {
                Store = new Store(null) { Guid = storeGuid},
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
                    ProductGuid = Guid.NewGuid(),
                    Amount = 5,
                    Price = 50
                },
                new()
                {
                    ProductGuid = Guid.NewGuid(),
                    Amount = 5,
                    Price = 50
                },
                new()
                {
                    ProductGuid = Guid.NewGuid(),
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
                    ProductGuid = Guid.NewGuid(),
                    Amount = 11,
                    Price = 50
                },
                new()
                {
                    ProductGuid = Guid.NewGuid(),
                    Amount = 11,
                    Price = 50
                },
                new()
                {
                    ProductGuid = Guid.NewGuid(),
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
        
        public static PurchaseDto GetPurchaseWithSingleProductWithAmountOf1(Guid userGuid, Guid productGuid, Guid storeGuid)
        {
            var productDto = new ProductDto
            {
                Guid = productGuid,
            };

            var purchaseDto = new PurchaseDto
            {
                BuyerGuid = userGuid,
                StorePurchases = new List<StorePurchaseDto>
                {
                    new()
                    {
                        StoreGuid = storeGuid,
                        PurchaseProducts = new List<PurchaseProductDto>
                        {
                            new()
                            {
                                Amount = 1,
                                ProductGuid = productDto.Guid
                            }
                        }
                    }
                }
            };

            return purchaseDto;
        }
        
        public static Store CreateStoreObject(string storeName)
        {
            return new(null) { StoreName = storeName };
        }

        public static Store CreateStoreObject(string storeName,Guid guid)
        {
            return new(null) { StoreName = storeName , Guid = guid};
        }

        public static User CreateUserObject(string name)
        {
            return new() { Name = name };
        }

        public static Product CreateProductObject(string name)
        {
            return new() {Name = name};
        }

        public static PurchaseProduct CreatePurchaseProductObject(Product p)
        {
            return new()
            {
                Product = p
            };
        }

        public static StorePurchase CreateStorePurchaseObject(Store store, User user, PurchaseProduct product)
        {
            return new ()
            {
                PurchaseProducts = new List<PurchaseProduct>{product},
                Buyer = user,
                Store = store,
                TotalPrice = 5
                
            };
        }

        public static StorePurchase CreateStorePurchaseObject(Store store,List<PurchaseProduct> products,Guid guid)
        {
            return new()
            {
                PurchaseProducts = products,
                Store = store,
                TotalPrice = 5,
                Guid = guid

            };
        }

        public static Purchase CreatePurchaseObject(StorePurchase sp,User buyer)
        {
            return new()
            {
                Buyer = buyer,
                StorePurchases = new List<StorePurchase> {sp}
            };
        }
        
        public static StoreOwnership CreateStoreOwnershipObject(Store store, User user)
        {
            return new() { Store = store, User = user };

        }
        
        public static StoreManagement CreateStoreManagementObject(Store store, User user)
        {
            return new() { Store = store, User = user, Permissions = new StoreManagementPermissions()};

        }
        
        
        public static User GetUserData(string fName, string lName, string uname)
        {
            return new() { Name = fName, LastName = lName, UserName = uname };
        }

        public static Store GetStoreData(string name)
        {
            return new(null) { StoreName = name };
        }

        public static StoreManagement GetStoreManagementData(User u, Store s)
        {
            return new() { User = u, Store = s };
        }

        public static StoreOwnership GetStoreOwnershipData(User u, Store s)
        {
            return new() { User = u, Store = s };
        }

        public static StoreManagementPermissions GetStoreManagementPermissionData(bool flag)
        {
            return new() { CanAddProduct = flag };
        }
        
    }
}