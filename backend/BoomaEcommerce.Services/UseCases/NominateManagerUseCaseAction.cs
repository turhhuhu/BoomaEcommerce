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
    public class NominateManagerUseCaseAction : UseCaseAction
    {
        [JsonRequired]
        public string UserToNominateLabel { get; set; }
        [JsonRequired]
        public string OwnershipLabel { get; set; }
        
        
        public StoreManagementPermissionsDto ManagerPermissions {get; set; }

        public NominateManagerUseCaseAction(IUseCaseAction next, IServiceProvider sp, IHttpContextAccessor accessor) :
            base(next, sp, accessor)
        {
        }

        public NominateManagerUseCaseAction(IServiceProvider sp, IHttpContextAccessor accessor) : base(sp, accessor)
        {

        }

        public NominateManagerUseCaseAction()
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

            await storeService.NominateNewStoreManagerAsync(storeOwnership.Guid, new StoreManagementDto
            {
                User = userToNominate,
                Store = storeOwnership.Store,
                Permissions = ManagerPermissions ?? new StoreManagementPermissionsDto()
            });

            await Next(dict,claims);
        }
    }
}