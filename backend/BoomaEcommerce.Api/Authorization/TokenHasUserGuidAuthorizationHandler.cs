using BoomaEcommerce.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoomaEcommerce.Api.Authorization
{
    public class TokenHasUserGuidAuthorizationHandler : AuthorizationHandler<TokenHasUserGuidRequirement>
    {

        public TokenHasUserGuidAuthorizationHandler()
        {
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TokenHasUserGuidRequirement requirement)
        {
            if (context.User.TryGetUserGuid(out _))
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
