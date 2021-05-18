using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Domain.PurchasePolicy.Operators
{
    public class ConditionPolicyOperator : PolicyOperator
    {

        public override PolicyResult CheckPolicy(User user, ShoppingBasket basket, params PurchasePolicy[] policies)
        {
            if (policies.Length != 2)
            {
                throw new InvalidOperationException("Condition operator provided amount of policies different than 2.");
            }

            var ifPolicyResult = policies[0].CheckPolicy(user, basket);
            var thenPolicyResult = policies[1].CheckPolicy(user, basket);
            if (ifPolicyResult.IsOk && !thenPolicyResult.IsOk)
            {
                return PolicyResult.CombineFail(
                    new List<PolicyResult>{ifPolicyResult, thenPolicyResult},
                    "If then following policy is met:",
                    "Then the following policy must be met:");
            }
            return PolicyResult.Ok();
        }

        public override PolicyResult CheckPolicy(StorePurchase purchase, params PurchasePolicy[] policies)
        {
            if (policies.Length != 2)
            {
                throw new InvalidOperationException("Condition operator provided amount of policies different than 2.");
            }

            var ifPolicyResult = policies[0].CheckPolicy(purchase);
            var thenPolicyResult = policies[1].CheckPolicy(purchase);
            if (ifPolicyResult.IsOk && !thenPolicyResult.IsOk)
            {
                return PolicyResult.CombineFail(
                    new List<PolicyResult> { ifPolicyResult, thenPolicyResult },
                    "If then following policy is met:",
                    "Then the following policy must be met:");
            }
            return PolicyResult.Ok();
        }
    }
}
