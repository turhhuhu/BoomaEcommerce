using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace BoomaEcommerce.Services.UseCases
{
    public class GetOwnershipUseCaseAction : UseCaseAction
    {
        [JsonRequired]
        public string StoreLabel { get; set; }
        [JsonRequired]
        public string UserLabel { get; set; }

        public GetOwnershipUseCaseAction(IUseCaseAction next, IServiceProvider sp, IHttpContextAccessor accessor) :
            base(next, sp, accessor)
        {
        }

        public GetOwnershipUseCaseAction(IServiceProvider sp, IHttpContextAccessor accessor) : base(sp, accessor)
        {

        }

        public GetOwnershipUseCaseAction()
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

            var userObj = dict[UserLabel];
            if (userObj is not UserDto user)
            {
                throw new ArgumentException(nameof(userObj));
            }

            using var scope = Sp.CreateScope();

            var storeService = scope.ServiceProvider.GetRequiredService<IStoresService>();

            var ownership = await storeService.GetStoreOwnerShipAsync(user.Guid, store.Guid);
            
            dict.Add(Label,ownership);
            scope.Dispose();
            await Next(dict, claims);
        }
    }
}
