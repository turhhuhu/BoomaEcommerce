using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Domain.Policies.Operators;

namespace BoomaEcommerce.Domain.Policies
{
    public class CompositePolicy : Policy
    {
        public PolicyOperator Operator { get; set; }
        private readonly List<Policy> _subPolicies;

        public CompositePolicy(PolicyOperator op)
        {
            Operator = op;
            op.Level = Level;
            op.ErrorPrefix = ErrorPrefix;
            _subPolicies = new List<Policy>();
        }

        public void AddPolicy(Policy policy)
        {
            policy.SetPolicyNode(Level + 1, ErrorPrefix + "\t");
            _subPolicies.Add(policy);
        }
        public void RemovePolicy(Guid policyId)
        {
            var policy = _subPolicies.FirstOrDefault(p => p.Guid == policyId);
            if (policy != null)
            {
                _subPolicies.Remove(policy);
            }
        }

        public IEnumerable<Policy> GetSubPolicies()
        {
            return _subPolicies.ToList();
        }

        protected internal override void SetPolicyNode(int level, string prefix)
        {
            base.SetPolicyNode(level, prefix);
            Operator.ErrorPrefix = prefix;
            Operator.Level = level;
            _subPolicies.ForEach(p => p.SetPolicyNode(Level + 1, prefix + "\t"));
        }

        public override PolicyResult CheckPolicy(User user, ShoppingBasket basket)
        {
            return Operator.CheckPolicy(user, basket, _subPolicies.ToArray());
        }

        public override PolicyResult CheckPolicy(StorePurchase purchase)
        {
            return Operator.CheckPolicy(purchase, _subPolicies.ToArray());
        }
    }
}
