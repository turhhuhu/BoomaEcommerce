using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Domain.PurchasePolicy.Operators
{
    public class AndPolicyOperator : PolicyOperator
    {
        private readonly string _failMessage;
        public AndPolicyOperator()
        {
            _failMessage = "All of the following policies must be met:";
        }
        public override PolicyResult CheckPolicy(User user, ShoppingBasket basket, IEnumerable<PurchasePolicy> policies)
        {
            var fails = policies
                .Select(p => p.CheckPolicy(user, basket))
                .Where(r => !r.IsOk)
                .ToList();

            return fails.Any()
                ? PolicyResult.CombineFail(fails, ErrorPrefix + _failMessage)
                : PolicyResult.Ok();
        }

        public override PolicyResult CheckPolicy(StorePurchase purchase, IEnumerable<PurchasePolicy> policies)
        {
            var fails = policies
                .Select(p => p.CheckPolicy(purchase))
                .Where(r => !r.IsOk)
                .ToList();

            return fails.Any()
                ? PolicyResult.CombineFail(fails, ErrorPrefix + _failMessage)
                : PolicyResult.Ok();
        }
    }
}
