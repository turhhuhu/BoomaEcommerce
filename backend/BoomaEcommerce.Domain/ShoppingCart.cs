using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Core;

namespace BoomaEcommerce.Domain
{
    public class ShoppingCart : BaseEntity
    {
        public ShoppingCart(User user) : base(user.Id)
        {
            User = user;
            ShoppingBaskets = new HashSet<ShoppingBasket>(new ShoppingBasketSameStoreGuid());
        }

        private ShoppingCart()
        {
            ShoppingBaskets = new HashSet<ShoppingBasket>(new ShoppingBasketSameStoreGuid());
        }

        public User User { get; set; }


        public ISet<ShoppingBasket> ShoppingBaskets { get; set; }

        public bool AddShoppingBasket(ShoppingBasket shoppingBasket)
        {
            return shoppingBasket is not null && ShoppingBaskets.Add(shoppingBasket);
        }
        public bool RemoveShoppingBasket(Guid storeGuid)
        {
            return ShoppingBaskets.Remove(new ShoppingBasket {Store = new Store {Guid = storeGuid}});
        }
    }
}
