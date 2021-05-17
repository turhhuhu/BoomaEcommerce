using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Core;

namespace BoomaEcommerce.Domain.PurchasePolicy.Operators
{
    public class OrPolicyOperator : PolicyOperator
    {
        private const string FailMessage = "One of the following policies must be met:";
        public override PolicyResult CheckPolicy(User user, ShoppingBasket basket, params PurchasePolicy[] policies)
        {
            var (okResults, failResults) = policies
                .Select(p => p.CheckPolicy(user, basket))
                .Split(res => res.IsOk);

            return okResults.Any()
                ? PolicyResult.Ok()
                : PolicyResult.CombineFail(failResults, ErrorPrefix + FailMessage);
        }

        public override PolicyResult CheckPolicy(StorePurchase purchase, params PurchasePolicy[] policies)
        {
            var (okResults, failResults) = policies
                .Select(p => p.CheckPolicy(purchase))
                .Split(res => res.IsOk);

            return okResults.Any()
                ? PolicyResult.Ok()
                : PolicyResult.CombineFail(failResults, ErrorPrefix + FailMessage);
        }
    }
}
