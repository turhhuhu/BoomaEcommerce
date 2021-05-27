using BoomaEcommerce.Core;
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
            var (okResults, failResults) = policies
                .Select(policy => policy.CheckPolicy(user, basket))
                .Split(res => res.IsOk);

            return okResults.Count() == 1
                ? PolicyResult.Ok()
                : PolicyResult.Fail();
        }

        public override PolicyResult CheckPolicy(StorePurchase purchase, params Policy[] policies)
        {
            var (okResults, failResults) = policies
                .Select(policy => policy.CheckPolicy(purchase))
                .Split(res => res.IsOk);

            return okResults.Count() == 1
                ? PolicyResult.Ok()
                : PolicyResult.Fail();
        }
    }
}
