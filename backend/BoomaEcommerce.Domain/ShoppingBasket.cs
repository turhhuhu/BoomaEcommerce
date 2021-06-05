using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Domain.Policies;

namespace BoomaEcommerce.Domain
{
    public class ShoppingBasket : BaseEntity
    {
        public Store Store { get; set; }
        public IDictionary<Guid, PurchaseProduct> PurchaseProducts { get; set; } = new ConcurrentDictionary<Guid, PurchaseProduct>();

        public PolicyResult CheckPolicyCompliance(User user, PurchaseProduct purchaseProduct)
        {
            var purchaseProducts = PurchaseProducts.ToDictionary(x => x.Key, x => x.Value);
            purchaseProducts.Add(purchaseProduct.Guid, purchaseProduct);
            return Store.CheckPolicy(user, new ShoppingBasket {Store = Store, PurchaseProducts = purchaseProducts});
        }

        public string ApplyDiscountCompliance(User user, PurchaseProduct purchaseProduct)
        {
            var purchaseProducts = PurchaseProducts.ToDictionary(x => x.Key, x => x.Value);
            purchaseProducts.Add(purchaseProduct.Guid, purchaseProduct);
            return Store.ApplyDiscount(user, new ShoppingBasket { Store = Store, PurchaseProducts = purchaseProducts });
        }
        public bool AddPurchaseProduct(PurchaseProduct purchaseProduct)
        {
            return purchaseProduct is not null && PurchaseProducts.TryAdd(purchaseProduct.Guid ,purchaseProduct);
        }
        public bool RemovePurchaseProduct(Guid purchaseProductGuid)
        {
            return PurchaseProducts.Remove(purchaseProductGuid, out _);
        }
    }
}
