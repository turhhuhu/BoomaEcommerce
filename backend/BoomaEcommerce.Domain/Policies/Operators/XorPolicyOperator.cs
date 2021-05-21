using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Domain.Policies.Operators
{
    public class XorPolicyOperator : PolicyOperator
    {
        public override PolicyResult CheckPolicy(User user, ShoppingBasket basket, params Policy[] policies)
        {
            throw new NotImplementedException();
        }

        public override PolicyResult CheckPolicy(StorePurchase purchase, params Policy[] policies)
        {
            throw new NotImplementedException();
        }
    }
}
