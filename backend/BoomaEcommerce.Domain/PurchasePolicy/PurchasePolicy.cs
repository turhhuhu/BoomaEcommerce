using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Core;

namespace BoomaEcommerce.Domain.PurchasePolicy
{
    public abstract class PurchasePolicy : BaseEntity
    {
        public abstract bool CheckPolicy(User user, ShoppingBasket basket);
        public abstract bool CheckPolicy(StorePurchase purchase);
    }
}
