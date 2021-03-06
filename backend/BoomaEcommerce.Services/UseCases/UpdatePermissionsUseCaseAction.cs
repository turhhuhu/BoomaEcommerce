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
    public class UpdatePermissionsUseCaseAction : UseCaseAction
    {
        [JsonRequired]
        public string ManagementLabel { get; set; }

        [JsonRequired]
        public StoreManagementPermissionsDto ManagerPermissions {get; set; }

        public UpdatePermissionsUseCaseAction(IUseCaseAction next, IServiceProvider sp, IHttpContextAccessor accessor) :
            base(next, sp, accessor)
        {
        }

        public UpdatePermissionsUseCaseAction(IServiceProvider sp, IHttpContextAccessor accessor) : base(sp, accessor)
        {

        }

        public UpdatePermissionsUseCaseAction()
        {
        }
        public override async Task NextAction(Dictionary<string,object> dict = null, ClaimsPrincipal claims = null)
        {
            if (dict is null)
            {
                throw new ArgumentException(nameof(dict));
            }

            var managementsoObj = dict[ManagementLabel];
            if (managementsoObj is not StoreManagementDto management)
            {
                throw new ArgumentException(nameof(managementsoObj));
            }

            ManagerPermissions.Guid = management.Guid;

            using var scope = Sp.CreateScope();

            var storeService = scope.ServiceProvider.GetRequiredService<IStoresService>();

            await storeService.UpdateManagerPermissionAsync(ManagerPermissions);
            scope.Dispose();
            await Next(dict,claims);
        }
    }
}