
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BoomaEcommerce.Data.EfCore.Repositories;
using BoomaEcommerce.Domain.Discounts;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.DTO.Discounts;
using BoomaEcommerce.Services.Products;
using BoomaEcommerce.Services.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace BoomaEcommerce.Services.UseCases
{
    public class GetAllProductsUseCaseAction : UseCaseAction
    {

        [JsonRequired]
        public string Category { get; set; }
        
        [JsonRequired]
        public string ProductName { get; set; }
        
        [JsonRequired]
        public decimal? Rating { get; set; }

        
        public GetAllProductsUseCaseAction(IUseCaseAction next, IServiceProvider sp, IHttpContextAccessor accessor) : base(next, sp, accessor)
        {
        }

        public GetAllProductsUseCaseAction(IServiceProvider sp, IHttpContextAccessor accessor) : base(sp, accessor)
        {
        }
        public GetAllProductsUseCaseAction()
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

            var products = await productService.GetAllProductsAsync(Category,ProductName,Rating);
            dict.Add(Label,products);
            scope.Dispose();
            await Next(dict, claims);
        }
    }
}