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
    public class CreateStoreProductUseCaseAction : UseCaseAction
    {
        [JsonRequired]
        public string ProductName { get; set; }

        [JsonRequired]
        public int ProductAmount { get; set; }
        
        [JsonRequired]
        public int ProductPrice { get; set; }
        
        [JsonRequired]
        public string StoreLabel { get; set; }
        
        public CreateStoreProductUseCaseAction(IUseCaseAction next, IServiceProvider sp, IHttpContextAccessor accessor) : base(next, sp, accessor)
        {
        }

        public CreateStoreProductUseCaseAction(IServiceProvider sp, IHttpContextAccessor accessor) : base(sp, accessor)
        {
        }
        public CreateStoreProductUseCaseAction()
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

            var product = await storeService.CreateStoreProductAsync(new ProductDto
            {
                StoreGuid = store.Guid,
                Amount = ProductAmount,
                Name = ProductName,
                Price = ProductPrice
                
            });
            dict.Add(Label,product);
            await Next(dict, claims);
        }
    }
}