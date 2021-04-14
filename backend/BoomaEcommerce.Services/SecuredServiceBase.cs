using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Services.Exceptions;
using Microsoft.AspNetCore.Authorization;

namespace BoomaEcommerce.Services
{
    public class SecuredServiceBase
    {
        protected readonly ClaimsPrincipal Claims;

        protected SecuredServiceBase(ClaimsPrincipal claims)
        {
            Claims = claims;
        }

        protected static ClaimsPrincipal ParseClaimsPrincipal(string token)
        {
            var tokenSecurityHandler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = tokenSecurityHandler.ReadJwtToken(token);
            var claimsPrincipal =
                new ClaimsPrincipal(new ClaimsIdentity(jwtSecurityToken.Claims));
            return claimsPrincipal;
        }

        protected void CheckAuthorized(MethodInfo method)
        {
            if (Claims.Identity?.IsAuthenticated == false || !Claims.HasClaim(claim => claim.Type == "guid"))
            {
                throw new UnAuthenticatedException();
            }

            var authAttributes = method.GetCustomAttributes<AuthorizeAttribute>(true);
            var roles = authAttributes
                .SelectMany(x => x.Roles?.Split(','))
                .Where(role => role != null);

            var isInRoles = roles.All(role => Claims.IsInRole(role));
            if (!isInRoles)
            {
                throw new UnAuthorizedException($"Roles are missing for user {Claims.GetUserGuid()} to use the resource.");
            }
        }
    }
}
