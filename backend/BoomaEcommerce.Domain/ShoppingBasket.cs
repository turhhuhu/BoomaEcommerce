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

        private IDictionary<Guid, PurchaseProduct> _purchaseProducts;

        public ShoppingBasket()
        {
            _purchaseProducts = new ConcurrentDictionary<Guid, PurchaseProduct>();
        }
        public ICollection<PurchaseProduct> PurchaseProducts
        {
            get => _purchaseProducts.Values;

            set => _purchaseProducts =
                new ConcurrentDictionary<Guid, PurchaseProduct>(value.ToDictionary(p => p.Guid));
        }


        public PolicyResult CheckPolicyCompliance(User user, PurchaseProduct purchaseProduct)
        {
            var purchaseProducts = _purchaseProducts.ToDictionary(x => x.Key, x => x.Value);
            purchaseProducts.Add(purchaseProduct.Guid, purchaseProduct);
            return Store.CheckPolicy(user, new ShoppingBasket {Store = Store, PurchaseProducts = purchaseProducts.Values});
        }

        public bool AddPurchaseProduct(PurchaseProduct purchaseProduct)
        {
            return purchaseProduct is not null && _purchaseProducts.TryAdd(purchaseProduct.Guid ,purchaseProduct);
        }
        public bool RemovePurchaseProduct(Guid purchaseProductGuid)
        {
            return _purchaseProducts.Remove(purchaseProductGuid, out _);
        }

        public bool Contains(Guid purchaseProductGuid)
        {
            return _purchaseProducts.ContainsKey(purchaseProductGuid);
        }
    }
}
