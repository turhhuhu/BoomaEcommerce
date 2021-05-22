using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Domain
{
    public struct PurchaseResult
    {
        public bool Success { get; set; }
        public List<StorePurchaseError> Errors { get; set; }

        public PurchaseResult(List<StorePurchaseError> errors)
        {
            Success = false;
            Errors = errors;
        }
        public PurchaseResult(bool state)
        {
            Success = state;
            Errors = new List<StorePurchaseError>();
        }
        public static PurchaseResult Fail(List<StorePurchaseError> failedPolicyResults)
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
    public class StorePurchaseError
    {
        public string Error { get; set; }
        public Guid StoreGuid { get; set; }
        public StorePurchaseError(Guid storeGuid, string error)
        {
            Error = error;
            StoreGuid = storeGuid;
        }
    }
}
