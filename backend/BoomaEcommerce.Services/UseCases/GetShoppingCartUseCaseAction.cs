using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Stores;
using BoomaEcommerce.Services.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace BoomaEcommerce.Services.UseCases
{
    public class GetShoppingCartUseCaseAction : UseCaseAction
    {
        [JsonRequired]
        public string UserLabel { get; set; }
        
        
        
        public GetShoppingCartUseCaseAction(IUseCaseAction next, IServiceProvider sp, IHttpContextAccessor accessor) :
            base(next, sp, accessor)
        {
        }

        public GetShoppingCartUseCaseAction(IServiceProvider sp, IHttpContextAccessor accessor) : base(sp, accessor)
        {

        }

        public GetShoppingCartUseCaseAction()
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

            var userService = scope.ServiceProvider.GetRequiredService<IUsersService>();

            var shoppingCart = await userService.GetShoppingCartAsync(user.Guid);
            dict.Add(Label,shoppingCart);
            scope.Dispose();
            await Next(dict,claims);
        }
    }
}