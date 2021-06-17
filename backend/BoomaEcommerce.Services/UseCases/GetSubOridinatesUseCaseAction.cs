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
    public class GetSubOridinatesUseCaseAction : UseCaseAction
    {
        [JsonRequired]
        public string OwnershipLabel { get; set; }
        
        public GetSubOridinatesUseCaseAction(IUseCaseAction next, IServiceProvider sp, IHttpContextAccessor accessor) : base(next, sp, accessor)
        {
        }

        public GetSubOridinatesUseCaseAction(IServiceProvider sp, IHttpContextAccessor accessor) : base(sp, accessor)
        {
        }
        public GetSubOridinatesUseCaseAction()
        {

        }
        public override async Task NextAction(Dictionary<string,object> dict = null, ClaimsPrincipal claims = null)
        {
            if (dict is null)
            {
                throw new ArgumentException(nameof(dict));
            }

            var ownershipObj = dict[OwnershipLabel];
            if (ownershipObj is not StoreOwnershipDto storeOwnership)
            {
                throw new ArgumentException(nameof(ownershipObj));
            }

            using var scope = Sp.CreateScope();

            var storeService = scope.ServiceProvider.GetRequiredService<IStoresService>();

            var subOridinates = await storeService.GetSubordinateSellersAsync(storeOwnership.Guid);
            
            dict.Add(Label,subOridinates);
            scope.Dispose();
            await Next(dict,claims);
            
        }
    }
}