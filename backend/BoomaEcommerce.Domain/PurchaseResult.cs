using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Core.Exceptions;

namespace BoomaEcommerce.Domain
{
    public struct PurchaseResult
    {
        public bool Success { get; set; }
        public List<PolicyError> Errors { get; set; }

        public bool IsPolicyFailure => !Success && Errors.Any();

        public PurchaseResult(List<PolicyError> errors)
        {
            Success = false;
            Errors = errors;
        }
        public PurchaseResult(bool state)
        {
            Success = state;
            Errors = new List<PolicyError>();
        }
        public static PurchaseResult Fail(List<PolicyError> failedPolicyResults)
        {
            return new PurchaseResult(failedPolicyResults);
        }
        public static PurchaseResult Fail()
        {
            return new PurchaseResult(false);
        }
        public static PurchaseResult Ok()
        {
            return new PurchaseResult(true);
        }
    }
}
