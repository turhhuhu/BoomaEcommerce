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
    public class DeleteProductUseCaseAction : UseCaseAction
    {
        
        [JsonRequired]
        public string ProductLabel { get; set; }

        public DeleteProductUseCaseAction(IUseCaseAction next, IServiceProvider sp, IHttpContextAccessor accessor) :
            base(next, sp, accessor)
        {
        }

        public DeleteProductUseCaseAction(IServiceProvider sp, IHttpContextAccessor accessor) : base(sp, accessor)
        {
        }

        public DeleteProductUseCaseAction()
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

            var storeService = scope.ServiceProvider.GetRequiredService<IStoresService>();

            await storeService.DeleteProductAsync(product.Guid);
            scope.Dispose();
            await Next(dict, claims);
        }
    }
}