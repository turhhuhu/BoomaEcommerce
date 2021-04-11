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
        public double TotalPrice { get; set; }

        public Task<bool> MakePurchase()
        {
            return PurchaseStoreProducts();
        }

        private async Task<bool> PurchaseStoreProducts()
        {
            if (!CanPurchase(StorePurchases))
            {
                return false;
            }

            var results = await Task.WhenAll(StorePurchases.Select(x => x.PurchaseProducts()));
            return results.All(x => x);
        }
        

        private bool CanPurchase(List<StorePurchase> storePurchases)
        {
            return StorePurchases.All(x => x.CanPurchase());
        }
    }

}
