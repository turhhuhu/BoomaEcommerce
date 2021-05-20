using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BoomaEcommerce.Domain.Policies.Operators
{
    public class AndPolicyOperator : PolicyOperator
    {
        private const string FailMessage = "All of the following policies must be met:";

        public override PolicyResult CheckPolicy(User user, ShoppingBasket basket, params Policy[] policies)
        {
            var fails = policies
                .Select(p => p.CheckPolicy(user, basket))
                .Where(r => !r.IsOk)
                .ToList();

            return fails.Any()
                ? PolicyResult.CombineFail(fails, ErrorPrefix + FailMessage)
                : PolicyResult.Ok();

        }

        public override PolicyResult CheckPolicy(StorePurchase purchase, params Policy[] policies)
        {
            var fails = policies
                .Select(p => p.CheckPolicy(purchase))
                .Where(r => !r.IsOk)
                .ToList();

            return fails.Any()
                ? PolicyResult.CombineFail(fails, ErrorPrefix + FailMessage)
                : PolicyResult.Ok();
        }
    }
}
