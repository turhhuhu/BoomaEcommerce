

using System.Collections.Generic;
using BoomaEcommerce.Core;

namespace BoomaEcommerce.Domain.Policies.Operators
{
    public abstract class PolicyOperator : BaseEntity
    {
        protected PolicyOperator()
        {
            Level = 0;
            ErrorPrefix = "";
        }

        public string ErrorPrefix { get; set; }

        public int Level { get; set; }
        public abstract PolicyResult CheckPolicy(User user, ShoppingBasket basket, params Policy[] policies);
        public abstract PolicyResult CheckPolicy(StorePurchase purchase, params Policy[] policies);
    }
}