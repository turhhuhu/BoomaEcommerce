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
        protected readonly ClaimsPrincipal ClaimsPrincipal;

        protected SecuredServiceBase(ClaimsPrincipal claimsPrincipal)
        {
            ClaimsPrincipal = claimsPrincipal;
        }

        protected static ClaimsPrincipal ParseClaimsPrincipal(string token)
        {
            var tokenSecurityHandler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = tokenSecurityHandler.ReadJwtToken(token);
            var claimsPrincipal =
                new ClaimsPrincipal(new ClaimsIdentity(jwtSecurityToken.Claims));
            return claimsPrincipal;
        }

        protected bool CheckRoleAuthorized(MethodInfo method)
        {
            var authAttributes = method.GetCustomAttributes<AuthorizeAttribute>(true);
            var roles = authAttributes
                .SelectMany(x => x.Roles?.Split(','))
                .Where(role => role != null);

            return roles.Any(role => ClaimsPrincipal.IsInRole(role));
        }

        protected void CheckAuthenticated()
        {
            if (ClaimsPrincipal.Identity?.IsAuthenticated == false ||
                !ClaimsPrincipal.HasClaim(claim => claim.Type == "guid"))
            {
                throw new UnAuthenticatedException();
            }
        }
    }
}
