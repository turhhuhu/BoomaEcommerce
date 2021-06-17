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
    public class RemoveStoreOwnerUseCaseAction : UseCaseAction
    {
        [JsonRequired]
        public string UserToRemoveLabel { get; set; }
        [JsonRequired]
        public string UserToRemoveFromLabel { get; set; }
        
        
        public RemoveStoreOwnerUseCaseAction(IUseCaseAction next, IServiceProvider sp, IHttpContextAccessor accessor) :
            base(next, sp, accessor)
        {
        }

        public RemoveStoreOwnerUseCaseAction(IServiceProvider sp, IHttpContextAccessor accessor) : base(sp, accessor)
        {

        }

        public RemoveStoreOwnerUseCaseAction()
        {
        }
        public override async Task NextAction(Dictionary<string,object> dict = null, ClaimsPrincipal claims = null)
        {
            if (dict is null)
            {
                throw new ArgumentException(nameof(dict));
            }

            var UserToRemoveFromObj = dict[UserToRemoveFromLabel];
            if (UserToRemoveFromObj is not StoreOwnershipDto userToRemoveFrom)
            {
                throw new ArgumentException(nameof(UserToRemoveFromObj));
            }
            var UserToRemoveObj = dict[UserToRemoveLabel];
            if (UserToRemoveObj is not StoreOwnershipDto userToRemove)
            {
                throw new ArgumentException(nameof(UserToRemoveObj));
            }


            using var scope = Sp.CreateScope();

            var storeService = scope.ServiceProvider.GetRequiredService<IStoresService>();

            await storeService.RemoveStoreOwnerAsync(userToRemoveFrom.Guid, userToRemove.Guid);
            scope.Dispose();
            await Next(dict,claims);
        }
    }
}