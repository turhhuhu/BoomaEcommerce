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

namespace BoomaEcommerce.Services.UseCases
{
    public class GetOwnershipUseCaseAction : UseCaseAction
    {
        public Guid? StoreGuid { get; set; }
        public Guid? UserGuid { get; set; }

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
        public override async Task NextAction(object obj = null, ClaimsPrincipal claims = null)
        {
            if (obj is StoreDto storeDto)
            {
                StoreGuid ??= storeDto.Guid;
                UserGuid ??= claims.GetUserGuid();
            }

            using var scope = Sp.CreateScope();

            var storeService = scope.ServiceProvider.GetRequiredService<IStoresService>();

            var ownership = await storeService.GetStoreOwnerShipAsync(UserGuid!.Value, StoreGuid!.Value);

            await Next(ownership, claims);
        }
    }
}
