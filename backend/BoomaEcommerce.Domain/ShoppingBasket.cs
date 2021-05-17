using System;
using System.Collections.Concurrent;
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
        public ConcurrentDictionary<Guid, PurchaseProduct> PurchaseProducts { get; set; } = new();


        public bool AddPurchaseProduct(PurchaseProduct purchaseProduct)
        {
            return purchaseProduct is not null && PurchaseProducts.TryAdd(purchaseProduct.Guid ,purchaseProduct);
        }
        public bool RemovePurchaseProduct(Guid purchaseProductGuid)
        {
            return PurchaseProducts.TryRemove(purchaseProductGuid, out _);
        }
    }
}
