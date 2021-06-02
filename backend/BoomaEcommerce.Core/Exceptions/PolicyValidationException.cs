using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Core.Exceptions
{
    public class PolicyValidationException : Exception
    {
        public IEnumerable<StorePolicyError> PolicyErrors { get; set; }
        public PolicyValidationException(params StorePolicyError[] errors)
        {
            PolicyErrors = errors;
        }

        public PolicyValidationException(IEnumerable<StorePolicyError> errors)
        {
            PolicyErrors = errors;
        }
    }
    public class StorePolicyError
    {
        public string Error { get; set; }
        public Guid StoreGuid { get; set; }
        public StorePolicyError(Guid storeGuid, string error)
        {
            Error = error;
            StoreGuid = storeGuid;
        }
    }
}
