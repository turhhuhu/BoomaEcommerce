using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Core;

namespace BoomaEcommerce.Domain
{
    public class ShoppingCart : BaseEntity
    {
        public User User { get; set; }
        public List<ShoppingBasket> Baskets { get; set; } = new();

        public bool AddShoppingBasket(ShoppingBasket shoppingBasket)
        {
            if (shoppingBasket is null)
            {
                return false;
            }
            Baskets.Add(shoppingBasket);
            return true;
        }
        
        public bool RemoveShoppingBasket(Guid shoppingBasketGuid)
        {
            var shoppingBasket = Baskets.Find(x => x.Guid == shoppingBasketGuid);
            if (shoppingBasket is null)
            {
                return false;
            }
            Baskets.Remove(shoppingBasket);
            return true;
        }
    }
}
