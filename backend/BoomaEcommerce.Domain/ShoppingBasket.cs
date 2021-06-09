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

        public ShoppingBasket()
        {
            PurchaseProducts = new HashSet<PurchaseProduct>(new EqualityComparers.SameGuid<PurchaseProduct>());
        }
        public ISet<PurchaseProduct> PurchaseProducts { get; set; }

        public PolicyResult CheckPolicyCompliance(User user, PurchaseProduct purchaseProduct)
        {
            var purchaseProducts = PurchaseProducts.ToHashSet(new EqualityComparers.SameGuid<PurchaseProduct>());
            purchaseProducts.Add(purchaseProduct);
            return Store.CheckPolicy(user, new ShoppingBasket { Store = Store, PurchaseProducts = purchaseProducts });
        }
        public bool AddPurchaseProduct(PurchaseProduct purchaseProduct)
        {
            return purchaseProduct is not null && PurchaseProducts.Add(purchaseProduct);
        }
        public bool RemovePurchaseProduct(Guid purchaseProductGuid)
        {
            var pp = new PurchaseProduct {Guid = purchaseProductGuid};
            var contains = PurchaseProducts.Contains(pp);

            return PurchaseProducts.Remove(pp);
        }

        public bool Contains(Guid purchaseProductGuid)
        {
            return PurchaseProducts.Contains(new PurchaseProduct { Guid = purchaseProductGuid});
        }
    }

    public class ShoppingBasketSameStoreGuid : EqualityComparer<ShoppingBasket>
    {
        public override bool Equals(ShoppingBasket x, ShoppingBasket y)
        {
            return x?.Store?.Guid == y?.Store?.Guid;
        }

        public override int GetHashCode(ShoppingBasket obj)
        {
            return obj?.Store?.Guid.GetHashCode() ?? obj.GetHashCode();
        }
    }
}
