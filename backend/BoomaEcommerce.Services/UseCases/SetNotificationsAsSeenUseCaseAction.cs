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
    public class SetNotificationsAsSeenUseCaseAction : UseCaseAction
    {
        [JsonRequired]
        public string UserLabel { get; set; }
        [JsonRequired]
        public string NotificationLabel { get; set; }
        
        
        
        public SetNotificationsAsSeenUseCaseAction(IUseCaseAction next, IServiceProvider sp, IHttpContextAccessor accessor) :
            base(next, sp, accessor)
        {
        }

        public SetNotificationsAsSeenUseCaseAction(IServiceProvider sp, IHttpContextAccessor accessor) : base(sp, accessor)
        {

        }

        public SetNotificationsAsSeenUseCaseAction()
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
            
            var NotificationObj = dict[NotificationLabel];
            if (NotificationObj is not NotificationDto notification)
            {
                throw new ArgumentException(nameof(NotificationObj));
            }


            using var scope = Sp.CreateScope();

            var userService = scope.ServiceProvider.GetRequiredService<IUsersService>();

            await userService.SetNotificationAsSeen(user.Guid,notification.Guid);

            scope.Dispose();
            await Next(dict,claims);
        }
    }
}