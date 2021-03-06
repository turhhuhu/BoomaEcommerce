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
    public class CreateDiscountSubPolicyUseCaseAction : UseCaseAction 
    {
        [JsonRequired]
        public string StoreLabel { get; set; }
        
        [JsonRequired]
        public string DiscountLabel { get; set; }
        
        [JsonRequired]
        public string PolicyLabel { get; set; }
        
        [JsonRequired]
        public PolicyDto NewPolicy { get; set; }
        
        
        
        public CreateDiscountSubPolicyUseCaseAction(IUseCaseAction next, IServiceProvider sp, IHttpContextAccessor accessor) : base(next, sp, accessor)
        {
        }

        public CreateDiscountSubPolicyUseCaseAction(IServiceProvider sp, IHttpContextAccessor accessor) : base(sp, accessor)
        {
        }
        public CreateDiscountSubPolicyUseCaseAction()
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
            
            var policyObj = dict[PolicyLabel];
            if (policyObj is not PolicyDto policy)
            {
                throw new ArgumentException(nameof(policyObj));
            }
            
            
            using var scope = Sp.CreateScope();

            var storeService = scope.ServiceProvider.GetRequiredService<IStoresService>();

            var addedSubPolicy = await storeService.CreateDiscountSubPolicy(store.Guid,discount.Guid,policy.Guid,NewPolicy);
            dict.Add(Label,addedSubPolicy);
            scope.Dispose();
            await Next(dict, claims);
        }
    }
}