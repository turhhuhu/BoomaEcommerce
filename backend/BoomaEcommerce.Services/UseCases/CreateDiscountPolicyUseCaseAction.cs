using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.DTO.Discounts;
using BoomaEcommerce.Services.DTO.Policies;
using BoomaEcommerce.Services.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace BoomaEcommerce.Services.UseCases
{
    public class CreateDiscountPolicyUseCaseAction : UseCaseAction
    {
        [JsonRequired]
        public string StoreLabel { get; set; }
        
        [JsonRequired]
        public string DiscountLabel { get; set; }
        
        [JsonRequired]
        public PolicyDto NewDiscount { get; set; }
        
        
        
        public CreateDiscountPolicyUseCaseAction(IUseCaseAction next, IServiceProvider sp, IHttpContextAccessor accessor) : base(next, sp, accessor)
        {
        }

        public CreateDiscountPolicyUseCaseAction(IServiceProvider sp, IHttpContextAccessor accessor) : base(sp, accessor)
        {
        }
        public CreateDiscountPolicyUseCaseAction()
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
            
            var discountObj = dict[DiscountLabel];
            if (discountObj is not DiscountDto discount)
            {
                throw new ArgumentException(nameof(discountObj));
            }
            
            
            using var scope = Sp.CreateScope();

            var storeService = scope.ServiceProvider.GetRequiredService<IStoresService>();

            var policy = await storeService.CreateDiscountPolicyAsync(store.Guid,discount.Guid,NewDiscount);
            dict.Add(Label, policy);
            scope.Dispose();
            await Next(dict, claims);
        }
    }
}