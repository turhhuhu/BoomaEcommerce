using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Core.Exceptions;

namespace BoomaEcommerce.Core
{
    public static class ClaimsExtensions
    {
        public static Guid GetUserGuid(this ClaimsPrincipal claimsPrincipal)
        {
            var guidString = claimsPrincipal.FindFirst(claim => claim.Type == "guid")?.Value;
            if (guidString == null)
            {
                throw new UnAuthenticatedException("User guid missing in token.");
            }
            return new Guid(guidString);
        }
        public static bool TryGetUserGuid(this ClaimsPrincipal claimsPrincipal, out Guid? userGuid)
        {
            var guidString = claimsPrincipal.FindFirst(claim => claim.Type == "guid")?.Value;
            if (guidString == null)
            {
                userGuid = null;
                return false;
            }
            userGuid = new Guid(guidString);
            return true;
        }
    }
}
