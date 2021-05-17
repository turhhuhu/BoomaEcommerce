

using System.Collections.Generic;

namespace BoomaEcommerce.Domain.PurchasePolicy.Operators
{
    public abstract class PolicyOperator
    {
        public abstract bool CheckPolicy(User user, ShoppingBasket basket, IEnumerable<PurchasePolicy> policies);
        public abstract bool CheckPolicy(StorePurchase purchase, IEnumerable<PurchasePolicy> policies);
    }
}