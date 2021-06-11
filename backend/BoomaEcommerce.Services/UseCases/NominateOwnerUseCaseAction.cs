using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace BoomaEcommerce.Services.UseCases
{
    public class NominateOwnerUseCaseAction : UseCaseAction
    {
        [JsonRequired]
        public string UserToNominateLabel { get; set; }
        [JsonRequired]
        public string OwnershipLabel { get; set; }
        
        public NominateOwnerUseCaseAction(IUseCaseAction next, IServiceProvider sp, IHttpContextAccessor accessor) :
            base(next, sp, accessor)
        {
        }

        public NominateOwnerUseCaseAction(IServiceProvider sp, IHttpContextAccessor accessor) : base(sp, accessor)
        {

        }

        public NominateOwnerUseCaseAction()
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
            
            var UserToNominateObj = dict[UserToNominateLabel];
            if (UserToNominateObj is not UserDto userToNominate)
            {
                throw new ArgumentException(nameof(ownershipObj));
            }

            using var scope = Sp.CreateScope();

            var storeService = scope.ServiceProvider.GetRequiredService<IStoresService>();

            await storeService.NominateNewStoreOwnerAsync(storeOwnership.Guid, new StoreOwnershipDto
            {
                User = new UserDto
                {
                    Guid = userToNominate.Guid,
                    UserName = userToNominate.Name
                },
                Store = new StoreDto
                {
                    Guid = storeOwnership.Store.Guid
                }
            });
            scope.Dispose();
            await Next(dict, claims);
        }
    }
}
