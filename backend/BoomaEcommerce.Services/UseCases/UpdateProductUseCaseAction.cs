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
        public int? NewAmount { get; set; }
        public decimal? NewPrice { get; set; }
        public decimal? NewRating { get; set; }
        public string? NewCategory { get; set; }
        public string? NewName { get; set; }

        
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

            
            using var scope = Sp.CreateScope();

            var storeService = scope.ServiceProvider.GetRequiredService<IStoresService>();

            var updatedProduct = await storeService.UpdateProductAsync(new ProductDto
            {
                Guid = product.Guid,
                Amount = NewAmount ?? product.Amount,
                Name = NewName ?? product.Name,
                Price = NewPrice ?? product.Price,
                Rating = NewRating ?? product.Rating,
                Category = NewCategory ?? product.Category
                
            });
            scope.Dispose();
            await Next(dict, claims);
        }
    }
}