using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Products;
using BoomaEcommerce.Services.Purchases;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace BoomaEcommerce.Services.UseCases
{
    public class GetPurchaseFinalPriceUseCaseAction : UseCaseAction
    {
        [JsonRequired]
        public PurchaseDto Purchase { get; set; }

        
        public GetPurchaseFinalPriceUseCaseAction(IUseCaseAction next, IServiceProvider sp, IHttpContextAccessor accessor) : base(next, sp, accessor)
        {
        }

        public GetPurchaseFinalPriceUseCaseAction(IServiceProvider sp, IHttpContextAccessor accessor) : base(sp, accessor)
        {
        }
        public GetPurchaseFinalPriceUseCaseAction()
        {

        }
        public override async Task NextAction(Dictionary<string,object> dict = null, ClaimsPrincipal claims = null)
        {
            if (dict is null)
            {
                dict = new Dictionary<string, object>();
            }

            using var scope = Sp.CreateScope();

            var purchaseService = scope.ServiceProvider.GetRequiredService<IPurchasesService>();

            var finalPrice = await purchaseService.GetPurchaseFinalPrice(Purchase);
            dict.Add(Label,finalPrice);
            scope.Dispose();
            await Next(dict, claims);
        }
    }
}