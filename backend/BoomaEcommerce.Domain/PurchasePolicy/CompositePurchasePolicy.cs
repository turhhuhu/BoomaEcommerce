using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Domain.PurchasePolicy.Operators;

namespace BoomaEcommerce.Domain.PurchasePolicy
{
    public class CompositePurchasePolicy : PurchasePolicy
    {
        private PolicyOperator Operator { get; set; }
        private readonly List<PurchasePolicy> _policies;

        public CompositePurchasePolicy(PolicyOperator op)
        {
            Operator = op;
            _policies = new List<PurchasePolicy>();
        }
        public void AddPolicy(CompositePurchasePolicy compositePurchasePolicy)
        {
            _policies.Add(compositePurchasePolicy);
        }
        public void RemovePolicy(Guid policyId)
        {
            var policy = _policies.FirstOrDefault(p => p.Guid == policyId);
            if (policy != null)
            {
                _policies.Remove(policy);
            }
        }
        public override bool CheckPolicy(User user, ShoppingBasket basket)
        {
            return Operator.CheckPolicy(user, basket, _policies);
        }

        public override bool CheckPolicy(StorePurchase purchase)
        {
            return Operator.CheckPolicy(purchase, _policies);
        }
    }
}
