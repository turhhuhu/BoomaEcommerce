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

        public override async Task NextAction(object obj = null, ClaimsPrincipal claims = null)
        {
            using var scope = Sp.CreateScope();

            var storeService = scope.ServiceProvider.GetRequiredService<IStoresService>();

            claims.TryGetUserGuid(out var userGuid);

            StoreToCreate.FounderUserGuid = userGuid ?? default;

            var store =  await storeService.CreateStoreAsync(StoreToCreate);

            await Next(store, claims);
        }
    }
}
