

using System.Collections.Generic;

namespace BoomaEcommerce.Domain.PurchasePolicy.Operators
{
    public abstract class PolicyOperator
    {
        protected PolicyOperator()
        {
            Level = 0;
            ErrorPrefix = "";
        }

        protected internal string ErrorPrefix { get; set; }

        protected internal int Level { get; set; }
        public abstract PolicyResult CheckPolicy(User user, ShoppingBasket basket, IEnumerable<PurchasePolicy> policies);
        public abstract PolicyResult CheckPolicy(StorePurchase purchase, IEnumerable<PurchasePolicy> policies);
    }
}