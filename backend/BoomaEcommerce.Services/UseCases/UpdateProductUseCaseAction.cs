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
    public class UpdateProductUseCaseAction : UseCaseAction
    {
        [JsonRequired]
        public string ProductLabel { get; set; }
        [JsonRequired]
        public string StoreLabel { get; set; }
        [JsonRequired]
        public ProductDto UpdatedProduct { get; set; }
        

        
        public UpdateProductUseCaseAction(IUseCaseAction next, IServiceProvider sp, IHttpContextAccessor accessor) : base(next, sp, accessor)
        {
        }

        public UpdateProductUseCaseAction(IServiceProvider sp, IHttpContextAccessor accessor) : base(sp, accessor)
        {
        }
        public UpdateProductUseCaseAction()
        {

        }

        public override async Task NextAction(Dictionary<string,object> dict = null, ClaimsPrincipal claims = null)
        {
            if (dict is null)
            {
                throw new ArgumentException(nameof(dict));
            }

            var productshipObj = dict[ProductLabel];
            if (productshipObj is not ProductDto product)
            {
                throw new ArgumentException(nameof(productshipObj));
            }
            var storeObj = dict[StoreLabel];
            if (storeObj is not StoreDto store)
            {
                throw new ArgumentException(nameof(storeObj));
            }

            UpdatedProduct.StoreGuid = store.Guid;
            UpdatedProduct.Guid = product.Guid;

            
            using var scope = Sp.CreateScope();

            var storeService = scope.ServiceProvider.GetRequiredService<IStoresService>();

            var updatedProduct = await storeService.UpdateProductAsync(UpdatedProduct);
            scope.Dispose();
            await Next(dict, claims);
        }
    }
}