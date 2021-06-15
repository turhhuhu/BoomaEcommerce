using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace BoomaEcommerce.Services.UseCases
{
    public class GetUserInfoUseCaseAction : UseCaseAction
    {
        
        [JsonRequired]
        public string UserLabel { get; set; }
        
      
        
        
        
        public GetUserInfoUseCaseAction(IUseCaseAction next, IServiceProvider sp, IHttpContextAccessor accessor) :
            base(next, sp, accessor)
        {
        }

        public GetUserInfoUseCaseAction(IServiceProvider sp, IHttpContextAccessor accessor) : base(sp, accessor)
        {

        }

        public GetUserInfoUseCaseAction()
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

            var userDetails = await userService.GetUserInfoAsync(user.Guid);
            dict.Add(Label,userDetails);
            scope.Dispose();
            await Next(dict,claims);
        }
    }
}