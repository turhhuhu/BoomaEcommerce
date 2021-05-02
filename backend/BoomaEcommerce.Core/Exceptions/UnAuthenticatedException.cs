using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Core.Exceptions
{
    public class UnAuthenticatedException : Exception
    {
        public UnAuthenticatedException() : base("The request does not contain a valid token to authenticate.")
        {

        }

        public UnAuthenticatedException(string message) : base(message)
        {
            
        }
    }
}
