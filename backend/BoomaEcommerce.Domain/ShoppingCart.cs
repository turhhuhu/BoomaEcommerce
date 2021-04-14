using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Core;

namespace BoomaEcommerce.Domain
{
    public class ShoppingCart : BaseEntity
    {
        public User User { get; set; }
        public ConcurrentDictionary<Guid, ShoppingBasket> StoreGuidToBaskets { get; set; } = new();
        public bool AddShoppingBasket(ShoppingBasket shoppingBasket)
        {
            return shoppingBasket is not null && StoreGuidToBaskets.TryAdd(shoppingBasket.Store.Guid, shoppingBasket);
        }
        public bool RemoveShoppingBasket(Guid shoppingBasketGuid)
        {
            return StoreGuidToBaskets.TryRemove(shoppingBasketGuid, out _);
        }
    }
}
