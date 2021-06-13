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
    public class CreateStoreUseCaseAction : UseCaseAction
    {
        
        [JsonRequired]
        public string UserLabel { get; set; }
        [JsonRequired]
        public StoreDto StoreToCreate { get; set; }

        public CreateStoreUseCaseAction(IUseCaseAction next, IServiceProvider sp, IHttpContextAccessor accessor) : base(next, sp, accessor)
        {
        }

        public CreateStoreUseCaseAction(IServiceProvider sp, IHttpContextAccessor accessor) : base(sp, accessor)
        {
            
        }

        public CreateStoreUseCaseAction()
        {

        }

        public override async Task NextAction(Dictionary<string,object> dict = null, ClaimsPrincipal claims = null)
        {
            
            if (dict is null)
            {
                throw new ArgumentException(nameof(dict));
            }

            var userObj = dict[UserLabel];
            if (userObj is not UserDto user)
            {
                throw new ArgumentException(nameof(userObj));
            }

            StoreToCreate.FounderUserGuid = user.Guid;
            
            using var scope = Sp.CreateScope();

            var storeService = scope.ServiceProvider.GetRequiredService<IStoresService>();

            //claims.TryGetUserGuid(out var userGuid);

            //StoreToCreate.FounderUserGuid = userGuid ?? default;

            var store = await storeService.CreateStoreAsync(StoreToCreate);

            dict.Add(Label,store);
            scope.Dispose();
            await Next(dict, claims);
        }
    }
}
