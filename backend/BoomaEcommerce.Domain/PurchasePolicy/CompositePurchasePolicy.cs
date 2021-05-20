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
            op.Level = Level;
            op.ErrorPrefix = ErrorPrefix;
            _policies = new List<PurchasePolicy>();
        }
        public void AddPolicy(PurchasePolicy purchasePolicy)
        {
            purchasePolicy.SetPolicyNode(Level + 1, ErrorPrefix + "\t");
            _policies.Add(purchasePolicy);
        }
        public void RemovePolicy(Guid policyId)
        {
            var policy = _policies.FirstOrDefault(p => p.Guid == policyId);
            if (policy != null)
            {
                _policies.Remove(policy);
            }
        }

        protected internal override void SetPolicyNode(int level, string prefix)
        {
            base.SetPolicyNode(level, prefix);
            Operator.ErrorPrefix = prefix;
            Operator.Level = level;
            _policies.ForEach(p => p.SetPolicyNode(Level + 1, prefix + "\t"));
        }

        public override PolicyResult CheckPolicy(User user, ShoppingBasket basket)
        {
            return Operator.CheckPolicy(user, basket, _policies.ToArray());
        }

        public override PolicyResult CheckPolicy(StorePurchase purchase)
        {
            return Operator.CheckPolicy(purchase, _policies.ToArray());
        }
    }
}
