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
    public class CreateStoreUseCaseAction : UseCaseAction
    {
        public IHttpContextAccessor Accessor { get; set; }
        public StoreDto StoreToCreate { get; set; }

        public CreateStoreUseCaseAction(IUseCaseAction next, IServiceProvider sp) : base(next, sp)
        {
        }

        public CreateStoreUseCaseAction()
        {

        }

        public override async Task NextAction(ClaimsPrincipal claims = null)
        {

            Accessor.HttpContext = claims != null
                ? new DefaultHttpContext {User = claims} 
                : null;

            using var scope = Sp.CreateScope();
            var storeService = scope.ServiceProvider.GetRequiredService<IStoresService>();

            claims.TryGetUserGuid(out var userGuid);

            StoreToCreate.FounderUserGuid = userGuid ?? default;

            await storeService.CreateStoreAsync(StoreToCreate);

            await Next(claims);

            Accessor.HttpContext = null;
        }
    }
}
