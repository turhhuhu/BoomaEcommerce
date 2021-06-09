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

namespace BoomaEcommerce.Services.UseCases
{
    public class NominateOwnerUseCaseAction : UseCaseAction
    {
        public Guid? NewOwnerUserGuid { get; set; }
        public string NewOwnerUserName { get; set; }

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

        public override async Task NextAction(object obj = null, ClaimsPrincipal claims = null)
        {

            if (obj is not StoreOwnershipDto storeOwnership)
            {
                throw new ArgumentException(nameof(obj));
            }

            using var scope = Sp.CreateScope();

            var storeService = scope.ServiceProvider.GetRequiredService<IStoresService>();

            await storeService.NominateNewStoreOwnerAsync(storeOwnership.Guid, new StoreOwnershipDto
            {
                User = new UserDto
                {
                    Guid = NewOwnerUserGuid ?? default,
                    UserName = NewOwnerUserName
                },
                Store = new StoreDto
                {
                    Guid = storeOwnership.Store.Guid
                }
            });

            await Next(claims);
        }
    }
}
