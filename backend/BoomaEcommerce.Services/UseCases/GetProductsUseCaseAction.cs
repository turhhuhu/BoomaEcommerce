using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace BoomaEcommerce.Services.UseCases
{
    public class GetProductsUseCaseAction : UseCaseAction
    {
        [JsonRequired]
        public string StoreLabel { get; set; }
        
        public GetProductsUseCaseAction(IUseCaseAction next, IServiceProvider sp, IHttpContextAccessor accessor) : base(next, sp, accessor)
        {
        }

        public GetProductsUseCaseAction(IServiceProvider sp, IHttpContextAccessor accessor) : base(sp, accessor)
        {
        }
        public GetProductsUseCaseAction()
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
            
            using var scope = Sp.CreateScope();

            var storeService = scope.ServiceProvider.GetRequiredService<IStoresService>();

            var products = await storeService.GetProductsFromStoreAsync(store.Guid);
            
            dict.Add(Label,products);
            scope.Dispose();
            await Next(dict, claims);
        }
    }
}