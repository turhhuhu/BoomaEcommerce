using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Services.Authentication;
using BoomaEcommerce.Services.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace BoomaEcommerce.Services.UseCases
{
    public class LoginUseCaseAction : UseCaseAction
    {

        [JsonRequired]
        public string UserName { get; set; }

        [JsonRequired]
        public string Password { get; set; }

        [JsonIgnore]
        public JwtSettings JwtSettings { get; set; }

        public LoginUseCaseAction(IUseCaseAction next, JwtSettings jwtSettings, IServiceProvider sp, IHttpContextAccessor accessor) : base(next, sp, accessor)
        {
            JwtSettings = jwtSettings;
        }

        public LoginUseCaseAction(JwtSettings jwtSettings, IServiceProvider sp, IHttpContextAccessor accessor) : base(sp, accessor)
        {
            JwtSettings = jwtSettings;
        }
        public LoginUseCaseAction()
        {
            
        }

        public override async Task NextAction(object obj = null, ClaimsPrincipal claims = null)
        {

            using var scope = Sp.CreateScope();
            var authService = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();

            var loginRes = await authService.LoginAsync(UserName, Password);

            var claimsPrincipal = SecuredServiceBase.ValidateToken(loginRes.Token, JwtSettings.Secret);

            await Next(obj, claimsPrincipal);
        }
    }
}
