#nullable enable
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
using BoomaEcommerce.Core.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace BoomaEcommerce.Services
{
    public class SecuredServiceBase
    {
        protected readonly ClaimsPrincipal? ClaimsPrincipal;

        public SecuredServiceBase()
        {
            
        }
        protected SecuredServiceBase(ClaimsPrincipal claimsPrincipal)
        {
            ClaimsPrincipal = claimsPrincipal;
        }

        protected static ClaimsPrincipal ValidateToken(string token, string secret)
        {
            var tokenSecurityHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);

            var claims = tokenSecurityHandler.ValidateToken(token,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = true,
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ClockSkew = TimeSpan.Zero
                }, out _);

            return claims;
        }

        protected bool CheckRoleAuthorized(MethodInfo? method)
        {
            if (ClaimsPrincipal == null || method == null)
            {
                return false;
            }

            var authAttributes = method.GetCustomAttributes<AuthorizeAttribute>(true);
            var roles = authAttributes
                .SelectMany(att => att.Roles?.Split(',') ?? Enumerable.Empty<string>());

            return roles.Any(role => ClaimsPrincipal.IsInRole(role));
        }

        protected void CheckAuthenticated()
        {

            if (ClaimsPrincipal?.Identity?.IsAuthenticated == false ||
                ClaimsPrincipal?.HasClaim(claim => claim.Type == "guid") == false)
            {
                throw new UnAuthenticatedException();
            }
        }
    }
}
