using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Core.Exceptions
{
    public class PolicyValidationException : Exception
    {
        public IEnumerable<PolicyError> PolicyErrors { get; set; }
        public PolicyValidationException(params PolicyError[] errors)
        {
            PolicyErrors = errors;
        }

        public PolicyValidationException(IEnumerable<PolicyError> errors)
        {
            PolicyErrors = errors;
        }
    }
    public class PolicyError
    {
        public string Error { get; set; }
        public Guid? StoreGuid { get; set; }
        public PolicyError(Guid storeGuid, string error)
        {
            Error = error;
            StoreGuid = storeGuid;
        }

        public PolicyError(string error)
        {
            Error = error;
        }
    }
}
