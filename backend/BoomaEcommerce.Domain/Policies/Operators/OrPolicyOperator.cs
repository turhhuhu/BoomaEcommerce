using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Core;

namespace BoomaEcommerce.Domain.Policies.Operators
{
    public class OrPolicyOperator : PolicyOperator
    {
        private const string FailMessage = "One of the following policies must be met:";
        public override PolicyResult CheckPolicy(User user, ShoppingBasket basket, params Policy[] policies)
        {
            var (okResults, failResults) = policies
                .Select(p => p.CheckPolicy(user, basket))
                .Split(res => res.IsOk);

            return okResults.Any()
                ? PolicyResult.Ok()
                : PolicyResult.CombineFails(failResults, ErrorPrefix + FailMessage);
        }

        public override PolicyResult CheckPolicy(StorePurchase purchase, params Policy[] policies)
        {
            var (okResults, failResults) = policies
                .Select(p => p.CheckPolicy(purchase))
                .Split(res => res.IsOk);

            return okResults.Any()
                ? PolicyResult.Ok()
                : PolicyResult.CombineFails(failResults, ErrorPrefix + FailMessage);
        }
    }
}
