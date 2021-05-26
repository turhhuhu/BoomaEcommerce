using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Domain.Policies;

namespace BoomaEcommerce.Domain
{
    public class StorePurchase : BaseEntity
    {
        public List<PurchaseProduct> PurchaseProducts { get; set; }
        public User Buyer { get; set; }
        public Store Store { get; set; }
        public decimal TotalPrice { get; set; }

        public async Task<bool> PurchaseAllProducts()
        {
            var orderedProductsPurchases = PurchaseProducts
                .OrderBy( x => x.Product.Id).ToList();
            
            await LockProducts(orderedProductsPurchases);

            if (!CanPurchase())
            {
                return false;
            }
            
            var res = orderedProductsPurchases.All(x => x.Purchase());
            
            ReleaseProducts(orderedProductsPurchases);
            return res;
        }

        public bool CanPurchase()
        {
            var orderedProductsPurchases = PurchaseProducts
                .OrderBy( x => x.Product.Id).ToList();
            return orderedProductsPurchases.All(x => x.ValidatePurchase());
        }

        internal PolicyResult CheckPolicyCompliance()
        {
            return Store.CheckPolicy(this);
        }

        private static void ReleaseProducts(IEnumerable<PurchaseProduct> orderedProductsPurchases)
        {
            foreach (var productsPurchase in orderedProductsPurchases)
            {
                productsPurchase.Product.ProductLock.Release();
            }
        }

        private static async Task LockProducts(IEnumerable<PurchaseProduct> orderedProductsPurchases)
        {
            foreach (var productsPurchase in orderedProductsPurchases)
            {
                await productsPurchase.Product.ProductLock.WaitAsync();
            }
        }

        public bool ValidatePrice()
        {
            var calculateTotalPrice = 0.0m;
            foreach (var productsPurchase in PurchaseProducts)
            {
                if (!productsPurchase.ValidatePrice())
                {
                    return false;
                }

                calculateTotalPrice += productsPurchase.Price;
            }

            return TotalPrice == calculateTotalPrice;
        }
    }
}
