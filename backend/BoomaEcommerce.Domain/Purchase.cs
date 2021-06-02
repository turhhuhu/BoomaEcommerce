using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Core.Exceptions;
using BoomaEcommerce.Domain.Policies;

namespace BoomaEcommerce.Domain
{
    public class Purchase : BaseEntity
    {
        public List<StorePurchase> StorePurchases { get; set; }
        public User Buyer { get; set; }
        public decimal TotalPrice { get; set; }

        public Task<PurchaseResult> MakePurchase()
        {
            return !ValidatePrice() ? Task.FromResult(PurchaseResult.Fail()) : PurchaseStoreProducts();
        }

        public bool ValidatePrice()
        {
            var calculatedTotalPrice = 0.0m;
            foreach (var storePurchase in StorePurchases)
            {
                if (!storePurchase.ValidatePrice())
                {
                    return false;
                }

                calculatedTotalPrice += storePurchase.TotalPrice;
            }

            return TotalPrice == calculatedTotalPrice;
        }

        private async Task<PurchaseResult> PurchaseStoreProducts()
        {
            var failedPolicyResults = StorePurchases
                .Select(sp => (sp.Store.Guid, sp.CheckPolicyCompliance()))
                .Where(result => !result.Item2.IsOk)
                .Select(res => new PolicyError(res.Item1, res.Item2.PolicyError))
                .ToList();

            if (failedPolicyResults.Any())
            {
                return PurchaseResult.Fail(failedPolicyResults);
            }

            if (!CanPurchase())
            {
                return PurchaseResult.Fail();
            }
            var results = await Task.WhenAll(StorePurchases.Select(x => x.PurchaseAllProducts()));
            return results.All(x => x)
                ? PurchaseResult.Ok() 
                : PurchaseResult.Fail();
        }
        

        private bool CanPurchase()
        {
            return StorePurchases.All(x => x.CanPurchase());
        }
    }

}
