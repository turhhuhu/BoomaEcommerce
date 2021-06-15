using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Products;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace BoomaEcommerce.Services.UseCases
{
    public class GetProductsFromStoreUseCaseAction : UseCaseAction
    {

    [JsonRequired] public string StoreLabel { get; set; }


    public GetProductsFromStoreUseCaseAction(IUseCaseAction next, IServiceProvider sp, IHttpContextAccessor accessor) :
            base(next, sp, accessor)
        {
        }

    public GetProductsFromStoreUseCaseAction(IServiceProvider sp, IHttpContextAccessor accessor) : base(sp, accessor)
        {
        }

    public GetProductsFromStoreUseCaseAction()
        {

        }

    public override async Task NextAction(Dictionary<string, object> dict = null, ClaimsPrincipal claims = null)
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

            var productService = scope.ServiceProvider.GetRequiredService<IProductsService>();

            var returnedProductS = await productService.GetProductsFromStoreAsync(store.Guid);
            dict.Add(Label, returnedProductS);
            scope.Dispose();
            await Next(dict, claims);
        }
    }
}
