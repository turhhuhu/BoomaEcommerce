using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BoomaEcommerce.Services.Authentication;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace BoomaEcommerce.Services.UseCases
{
    public class RegisterUseCaseAction : UseCaseAction
    {
        
        [JsonRequired]
        public string UserName { get; set; }

        [JsonRequired]
        public string Password { get; set; }
        
        public Guid? UserGuid { get; set; }

        [JsonIgnore]
        public JwtSettings JwtSettings { get; set; }

        public RegisterUseCaseAction(IUseCaseAction next, JwtSettings jwtSettings, IServiceProvider sp, IHttpContextAccessor accessor) : base(next, sp, accessor)
        {
            JwtSettings = jwtSettings;
        }

        public RegisterUseCaseAction(JwtSettings jwtSettings, IServiceProvider sp, IHttpContextAccessor accessor) : base(sp, accessor)
        {
            JwtSettings = jwtSettings;
        }
        public RegisterUseCaseAction()
        {
            
        }
        public override async Task NextAction(Dictionary<string,object> dict = null, ClaimsPrincipal claims = null)
        {
            using var scope = Sp.CreateScope();
            var authService = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();

            var user = new UserDto
            {
                Guid = UserGuid ?? Guid.NewGuid(),
                UserName = UserName
            };

            var registerRes = await authService.RegisterAsync(user, Password);

            var claimsPrincipal = SecuredServiceBase.ValidateToken(registerRes.Token, JwtSettings.Secret);

            if (dict is null)
            {
                dict = new Dictionary<string, object>();
            }
            
            dict.Add(Label,user);

            await Next(dict, claimsPrincipal);
        }
    }
}