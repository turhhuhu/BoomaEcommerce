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
    public class RemoveManagerUseCaseAction : UseCaseAction
    {
        [JsonRequired]
        public string UserToRemoveLabel { get; set; }
        [JsonRequired]
        public string UserToRemoveFromLabel { get; set; }
        
        
        public RemoveManagerUseCaseAction(IUseCaseAction next, IServiceProvider sp, IHttpContextAccessor accessor) :
            base(next, sp, accessor)
        {
        }

        public RemoveManagerUseCaseAction(IServiceProvider sp, IHttpContextAccessor accessor) : base(sp, accessor)
        {

        }

        public RemoveManagerUseCaseAction()
        {
        }
        public override async Task NextAction(Dictionary<string,object> dict = null, ClaimsPrincipal claims = null)
        {
            if (dict is null)
            {
                throw new ArgumentException(nameof(dict));
            }

            var UserToRemoveFromObj = dict[UserToRemoveFromLabel];
            if (UserToRemoveFromObj is not UserDto userToRemoveFrom)
            {
                throw new ArgumentException(nameof(UserToRemoveFromObj));
            }
            var UserToRemoveObj = dict[UserToRemoveLabel];
            if (UserToRemoveObj is not UserDto userToRemove)
            {
                throw new ArgumentException(nameof(UserToRemoveObj));
            }


            using var scope = Sp.CreateScope();

            var storeService = scope.ServiceProvider.GetRequiredService<IStoresService>();

            await storeService.RemoveManagerAsync(userToRemoveFrom.Guid, userToRemove.Guid);

            await Next(dict,claims);
        }
    }
}