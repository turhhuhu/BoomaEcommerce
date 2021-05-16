using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Domain.PurchasePolicy.Operators
{
    public class AndPolicyOperator : PolicyOperator
    {
        public override bool CheckPolicy(User user, ShoppingBasket basket, IEnumerable<PurchasePolicy> policies)
        {
            return policies.All(p => p.CheckPolicy(user, basket));
        }

        public override bool CheckPolicy(StorePurchase purchase, IEnumerable<PurchasePolicy> policies)
        {
            return policies.All(p => p.CheckPolicy(purchase));
        }
    }
}
