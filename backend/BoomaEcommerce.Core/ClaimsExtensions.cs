using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Core
{
    public static class ClaimsExtensions
    {
        public static Guid GetUserGuid(this ClaimsPrincipal claimsPrincipal)
        {
            var guidString = claimsPrincipal.FindFirst(claim => claim.Type == "guid")?.Value;
            if (guidString == null)
            {
                throw new UnauthorizedAccessException();
            }
            return new Guid(guidString);
        }
    }
}
