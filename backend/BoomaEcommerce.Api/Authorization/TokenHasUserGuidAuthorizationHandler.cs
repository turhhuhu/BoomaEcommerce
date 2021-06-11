using BoomaEcommerce.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Services.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;

namespace BoomaEcommerce.Api.Authorization
{
    public class TokenHasUserGuidAuthorizationHandler : AuthorizationHandler<TokenHasUserGuidRequirement>
    {
        private readonly JwtSettings _jwtSettings;

        public TokenHasUserGuidAuthorizationHandler(IOptions<JwtSettings> jwtOptions)
        {
            _jwtSettings = jwtOptions.Value;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TokenHasUserGuidRequirement requirement)
        {
            var expirationClaim = context.User?.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp)?.Value;
            if (expirationClaim == null)
            {
                context.Fail();
                return Task.CompletedTask;
            }
            var expirationDateUnix = long.Parse(expirationClaim);
            var expirationDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expirationDateUnix);

            if (context.User.TryGetUserGuid(out _) && expirationDateTimeUtc > DateTime.UtcNow)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
            return Task.CompletedTask;
        }
    }
}
