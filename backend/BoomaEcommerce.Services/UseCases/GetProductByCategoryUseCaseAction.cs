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
    public class GetProductByCategoryUseCaseAction : UseCaseAction
    {
        [JsonRequired]
        public string Category { get; set; }


        public GetProductByCategoryUseCaseAction(IUseCaseAction next, IServiceProvider sp, IHttpContextAccessor accessor) : base(next, sp, accessor)
        {
        }

        public GetProductByCategoryUseCaseAction(IServiceProvider sp, IHttpContextAccessor accessor) : base(sp, accessor)
        {
        }
        public GetProductByCategoryUseCaseAction()
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

            var productS = await productService.GetProductByCategoryAsync(Category);
            dict.Add(Label,productS);
            scope.Dispose();
            await Next(dict, claims);
        }
    }
}