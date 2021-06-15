using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Purchases;
using BoomaEcommerce.Services.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace BoomaEcommerce.Services.UseCases
{
    public class GetAllUserPurchaseHistoryUseCaseAction : UseCaseAction
    {
        [JsonRequired]
        public string UserLabel { get; set; }


        public GetAllUserPurchaseHistoryUseCaseAction(IUseCaseAction next, IServiceProvider sp, IHttpContextAccessor accessor) :
            base(next, sp, accessor)
        {
        }

        public GetAllUserPurchaseHistoryUseCaseAction(IServiceProvider sp, IHttpContextAccessor accessor) : base(sp, accessor)
        {

        }

        public GetAllUserPurchaseHistoryUseCaseAction()
        {
        }
        public override async Task NextAction(Dictionary<string,object> dict = null, ClaimsPrincipal claims = null)
        {
            if (dict is null)
            {
                throw new ArgumentException(nameof(dict));
            }

            var UserObj = dict[UserLabel];
            if (UserObj is not UserDto user)
            {
                throw new ArgumentException(nameof(UserObj));
            }
            

            using var scope = Sp.CreateScope();

            var storeService = scope.ServiceProvider.GetRequiredService<IPurchasesService>();

            var purchaseHistory = await storeService.GetAllUserPurchaseHistoryAsync(user.Guid);
            dict.Add(Label, purchaseHistory);
            scope.Dispose();
            await Next(dict,claims);
        }
    }
}