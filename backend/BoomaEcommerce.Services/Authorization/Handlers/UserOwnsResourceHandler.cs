using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;
using Microsoft.AspNetCore.Authorization;

namespace BoomaEcommerce.Services.Authorization.Handlers
{
    public class UserOwnsResourceHandler<TResource> 
        : AuthorizationHandler<UserOwnsResourceRequirement, TResource> 
        where TResource : IUserRelatedResource
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            UserOwnsResourceRequirement requirement, TResource resource)
        {
            var guid = context.User?.FindFirstValue("guid") ?? string.Empty;
            if (guid == resource.User.Guid.ToString())
            { 
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }

    public class UserOwnsResourceRequirement : IAuthorizationRequirement { }
}
