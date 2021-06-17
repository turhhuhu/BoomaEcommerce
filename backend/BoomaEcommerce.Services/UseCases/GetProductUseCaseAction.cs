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
    public class GetProductUseCaseAction : UseCaseAction
    {

        [JsonRequired]
        public string ProductLabel { get; set; }

        
        public GetProductUseCaseAction(IUseCaseAction next, IServiceProvider sp, IHttpContextAccessor accessor) : base(next, sp, accessor)
        {
        }

        public GetProductUseCaseAction(IServiceProvider sp, IHttpContextAccessor accessor) : base(sp, accessor)
        {
        }
        public GetProductUseCaseAction()
        {

        }
        public override async Task NextAction(Dictionary<string,object> dict = null, ClaimsPrincipal claims = null)
        {
            if (dict is null)
            {
                throw new ArgumentException(nameof(dict));
            }

            var productObj = dict[ProductLabel];
            if (productObj is not ProductDto product)
            {
                throw new ArgumentException(nameof(productObj));
            }
            
            using var scope = Sp.CreateScope();

            var productService = scope.ServiceProvider.GetRequiredService<IProductsService>();

            var returnedProduct = await productService.GetProductAsync(product.Guid);
            dict.Add(Label,returnedProduct);
            scope.Dispose();
            await Next(dict, claims);
        }
    }
}