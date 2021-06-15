using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BoomaEcommerce.Services.Authentication;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Purchases;
using BoomaEcommerce.Services.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace BoomaEcommerce.Services.UseCases
{
    public class LoadTestUseCaseAction : UseCaseAction
    {
        [JsonRequired]
        public string UserLabel { get; set; }
        [JsonRequired]
        public StoreDto StoreToCreate { get; set; }



        public LoadTestUseCaseAction(IUseCaseAction next, IServiceProvider sp, IHttpContextAccessor accessor) : base(next, sp, accessor)
        {
        }

        public LoadTestUseCaseAction(IServiceProvider sp, IHttpContextAccessor accessor) : base(sp, accessor)
        {
            
        }

        public LoadTestUseCaseAction()
        {

        }

        public override async Task NextAction(Dictionary<string,object> dict = null, ClaimsPrincipal claims = null)
        {
            
            if (dict is null)
            {
                throw new ArgumentException(nameof(dict));
            }

            var userObj = dict[UserLabel];
            if (userObj is not UserDto user)
            {
                throw new ArgumentException(nameof(userObj));
            }

            StoreToCreate.FounderUserGuid = user.Guid;
            
            using var scope = Sp.CreateScope();

            var storeService = scope.ServiceProvider.GetRequiredService<IStoresService>();
            var authService = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();
            var purchaseService = scope.ServiceProvider.GetRequiredService<IPurchasesService>();

            
            

            for (int i = 0; i < 1000; i++)
            {
                var registerRes = await authService.RegisterAsync(new UserDto
                {
                    Guid = Guid.NewGuid(),
                    UserName = "user" + i,
                }, "Password");
            }


            var store1 = new StoreDto();
            for (int i = 0; i < 1000; i++)
            {
                store1 = await storeService.CreateStoreAsync(StoreToCreate);
                for (int j = 0; j < 1000; j++)
                {
                    var product = await storeService.CreateStoreProductAsync(new ProductDto
                    {
                        Amount = 1,
                        Name = "product" + j,
                        Price = 1,
                        StoreGuid = store1.Guid
                    });
                }
                Console.WriteLine(i);
            }

            var p = await storeService.CreateStoreProductAsync(new ProductDto
            {
                Amount = 1001,
                Name = "productMillion",
                Price = 1,
                StoreGuid = store1.Guid
            });
            
            
            
            var purchaseProduct = new PurchaseProductDto
            {
                Amount = 1,
                Price = 1,
                ProductGuid = p.Guid
            };

            var ppList = new List<PurchaseProductDto>();
            ppList.Add(purchaseProduct);
            

            var storePurchase = new StorePurchaseDto
            {
                BuyerGuid = user.Guid,
                TotalPrice = 1,
                StoreGuid = store1.Guid,
                PurchaseProducts = ppList

            };

            var storePurchaseList = new List<StorePurchaseDto>();
            storePurchaseList.Add(storePurchase);
            
            
            
            for (int i = 0; i < 1000; i++)
            {
                var finalPrice = await purchaseService.CreatePurchaseAsync(new PurchaseDetailsDto
                {
                    PaymentDetails =  new PaymentDetailsDto
                    {
                        CardNumber = 555555555,
                        Ccv = 111,
                        HolderName = "DDD",
                        Id = 11111,
                        Month = 2,
                        Year = 2021
                    } ,
                    Purchase = new PurchaseDto
                    {
                        UserBuyerGuid = user.Guid,
                        TotalPrice = 1,
                        StorePurchases = storePurchaseList,
                        Buyer = new BasicUserInfoDto
                        {
                            UserName = "Benny",
                            DateOfBirth = new DateTime(2000,5,20)
                        }
                        
                    },
                    SupplyDetails = new SupplyDetailsDto
                    {
                        Address = "qaa",
                        City = "ssss",
                        Country = "ssss",
                        Name = "ksl",
                        Zip = 112334
                    }
                });
            }

            scope.Dispose();
            await Next(dict, claims);
        }
    }
}