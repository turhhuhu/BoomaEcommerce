using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Core;

namespace BoomaEcommerce.Domain
{
    public class StorePurchase : BaseEntity
    {
        public List<PurchaseProduct> ProductsPurchases { get; set; }
        public User Buyer { get; set; }
        public Store Store { get; set; }
        public double TotalPrice { get; set; }

        public async Task<bool> PurchaseProducts()
        {
            var orderedProductsPurchases = ProductsPurchases
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
            var orderedProductsPurchases = ProductsPurchases
                .OrderBy( x => x.Product.Id).ToList();
            return orderedProductsPurchases.All(x => x.ValidatePurchase());
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
    }
}
