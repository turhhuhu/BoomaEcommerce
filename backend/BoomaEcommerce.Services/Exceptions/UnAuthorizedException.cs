using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Services.Exceptions
{
    public class UnAuthorizedException : Exception
    {
        public UnAuthorizedException(string resource, Guid userGuid) 
            : base($"User '{userGuid}' is not authorized to use the resource {resource}")
        {

        }

        public UnAuthorizedException(Guid userGuid)
            : base($"User '{userGuid}' is not authorized to use the resource.")
        {

        }
        public UnAuthorizedException(string message)
            : base(message)
        {

        }
    }
}
