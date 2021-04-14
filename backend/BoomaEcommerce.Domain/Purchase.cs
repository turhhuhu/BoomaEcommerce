using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Core;

namespace BoomaEcommerce.Domain
{
    public class Purchase : BaseEntity
    {
        public List<StorePurchase> StorePurchases { get; set; }
        public User Buyer { get; set; }
        public decimal TotalPrice { get; set; }

        public Task<bool> MakePurchase()
        {
            return !ValidatePrice() ? Task.FromResult(false) : PurchaseStoreProducts();
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

        private async Task<bool> PurchaseStoreProducts()
        {
            if (!CanPurchase(StorePurchases))
            {
                return false;
            }

            var results = await Task.WhenAll(StorePurchases.Select(x => x.PurchaseAllProducts()));
            return results.All(x => x);
        }
        

        private bool CanPurchase(List<StorePurchase> storePurchases)
        {
            return StorePurchases.All(x => x.CanPurchase());
        }
    }

}
