using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Services.Exceptions
{
    public class UnAuthenticatedException : Exception
    {
        public UnAuthenticatedException() : base("The request does not contain a valid token to authenticate.")
        {

        }
    }
}
