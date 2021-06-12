using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace BoomaEcommerce.Services.UseCases
{
    public class UpdatePermissionsUseCaeAction : UseCaseAction
    {
        [JsonRequired]
        public string ManagementLabel { get; set; }

        [JsonRequired]
        public StoreManagementPermissionsDto ManagerPermissions {get; set; }

        public UpdatePermissionsUseCaeAction(IUseCaseAction next, IServiceProvider sp, IHttpContextAccessor accessor) :
            base(next, sp, accessor)
        {
        }

        public UpdatePermissionsUseCaeAction(IServiceProvider sp, IHttpContextAccessor accessor) : base(sp, accessor)
        {

        }

        public UpdatePermissionsUseCaeAction()
        {
        }
        public override async Task NextAction(Dictionary<string,object> dict = null, ClaimsPrincipal claims = null)
        {
            if (dict is null)
            {
                throw new ArgumentException(nameof(dict));
            }

            var managementsoObj = dict[ManagementLabel];
            if (managementsoObj is not StoreManagement manahement)
            {
                throw new ArgumentException(nameof(managementsoObj));
            }

            ManagerPermissions.Guid = manahement.Permissions.Guid;

            using var scope = Sp.CreateScope();

            var storeService = scope.ServiceProvider.GetRequiredService<IStoresService>();

            await storeService.UpdateManagerPermissionAsync(ManagerPermissions);
            scope.Dispose();
            await Next(dict,claims);
        }
    }
}