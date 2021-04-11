using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Core;

namespace BoomaEcommerce.Domain
{
    public class ShoppingBasket : BaseEntity
    {
        public Store Store { get; set; }
        public List<PurchaseProduct> PurchaseProducts { get; set; } = new();

        public bool AddPurchaseProduct(PurchaseProduct purchaseProduct)
        {
            if (purchaseProduct is null)
            {
                return false;
            }
            PurchaseProducts.Add(purchaseProduct);
            return true;
        }
        public bool RemovePurchaseProduct(Guid purchaseProductGuid)
        {
            var purchaseProduct = PurchaseProducts.Find(x => x.Guid == purchaseProductGuid);
            if (purchaseProduct is null)
            {
                return false;
            }
            PurchaseProducts.Remove(purchaseProduct);
            return true;
        }
    }
}
