using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BoomaEcommerce.Services.Products;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace BoomaEcommerce.Services.UseCases
{
    public class GetProductByNameUseCaseAction : UseCaseAction
    {
        [JsonRequired]
        public string ProductName { get; set; }


        public GetProductByNameUseCaseAction(IUseCaseAction next, IServiceProvider sp, IHttpContextAccessor accessor) : base(next, sp, accessor)
        {
        }

        public GetProductByNameUseCaseAction(IServiceProvider sp, IHttpContextAccessor accessor) : base(sp, accessor)
        {
        }
        public GetProductByNameUseCaseAction()
        {

        }
        public override async Task NextAction(Dictionary<string,object> dict = null, ClaimsPrincipal claims = null)
        {
            if (dict is null)
            {
                dict = new Dictionary<string, object>();
            }

            using var scope = Sp.CreateScope();

            var productService = scope.ServiceProvider.GetRequiredService<IProductsService>();

            var productS = await productService.GetProductByNameAsync(ProductName);
            dict.Add(Label,productS);
            scope.Dispose();
            await Next(dict, claims);
        }
    }
}