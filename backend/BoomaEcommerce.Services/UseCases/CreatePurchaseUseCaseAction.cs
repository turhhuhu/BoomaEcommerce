using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Purchases;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace BoomaEcommerce.Services.UseCases
{
    public class CreatePurchaseUseCaseAction : UseCaseAction
    {
        [JsonRequired]
        public PurchaseDetailsDto PurchaseDetails { get; set; }
        [JsonRequired]
        public string StoreLabel { get; set; }
        [JsonRequired]
        public string UserLabel { get; set; }
        [JsonRequired]
        public string ProductLabel { get; set; }
        
        public CreatePurchaseUseCaseAction(IUseCaseAction next, IServiceProvider sp, IHttpContextAccessor accessor) : base(next, sp, accessor)
        {
        }

        public CreatePurchaseUseCaseAction(IServiceProvider sp, IHttpContextAccessor accessor) : base(sp, accessor)
        {
        }
        public CreatePurchaseUseCaseAction()
        {

        }
        public override async Task NextAction(Dictionary<string,object> dict = null, ClaimsPrincipal claims = null)
        {
            if (dict is null)
            {
                throw new ArgumentException(nameof(dict));
            }

            var storeObj = dict[StoreLabel];
            if (storeObj is not StoreDto store)
            {
                throw new ArgumentException(nameof(storeObj));
            }

            var userObj = dict[UserLabel];
            if (userObj is not UserDto user)
            {
                throw new ArgumentException(nameof(userObj));
            }
            var productObj = dict[ProductLabel];
            if (productObj is not ProductDto product)
            {
                throw new ArgumentException(nameof(productObj));
            }

            PurchaseDetails.Purchase.UserBuyerGuid = user.Guid;
            PurchaseDetails.Purchase.StorePurchases[0].BuyerGuid = user.Guid;
            PurchaseDetails.Purchase.StorePurchases[0].StoreGuid = store.Guid;
            PurchaseDetails.Purchase.StorePurchases[0].BuyerGuid = user.Guid;
            PurchaseDetails.Purchase.StorePurchases[0].PurchaseProducts[0].ProductGuid = product.Guid;


            using var scope = Sp.CreateScope();

            var purchaseService = scope.ServiceProvider.GetRequiredService<IPurchasesService>();

            var finalPrice = await purchaseService.CreatePurchaseAsync(PurchaseDetails);
            dict.Add(Label,finalPrice);
            scope.Dispose();
            await Next(dict, claims);
        }
    }
}