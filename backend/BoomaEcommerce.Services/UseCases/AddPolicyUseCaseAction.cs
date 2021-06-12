using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.DTO.Policies;
using BoomaEcommerce.Services.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace BoomaEcommerce.Services.UseCases
{
    public class AddPolicyUseCaseAction : UseCaseAction
    {
        [JsonRequired]
        public string StoreLabel { get; set; }
        
        
        [JsonRequired]
        public string PolicyLabel { get; set; }
        
        [JsonRequired]
        public PolicyDto Policy { get; set; }
        
        
        
        public AddPolicyUseCaseAction(IUseCaseAction next, IServiceProvider sp, IHttpContextAccessor accessor) : base(next, sp, accessor)
        {
        }

        public AddPolicyUseCaseAction(IServiceProvider sp, IHttpContextAccessor accessor) : base(sp, accessor)
        {
        }
        public AddPolicyUseCaseAction()
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

            var policyObj = dict[PolicyLabel];
            if (policyObj is not PolicyDto policy)
            {
                throw new ArgumentException(nameof(policyObj));
            }
            
            
            using var scope = Sp.CreateScope();

            var storeService = scope.ServiceProvider.GetRequiredService<IStoresService>();

            var addedPolicy = await storeService.AddPolicyAsync(store.Guid,policy.Guid,Policy);
            dict.Add(Label,addedPolicy);
            scope.Dispose();
            await Next(dict, claims);
        }
    }
}