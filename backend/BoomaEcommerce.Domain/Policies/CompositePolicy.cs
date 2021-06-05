using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Domain.Policies.Operators;

namespace BoomaEcommerce.Domain.Policies
{
    public class CompositePolicy : MultiPolicy
    {
        public List<Policy> SubPolicies
        {
            get => _subPolicies.ToList();
            set => _subPolicies = value;
        }

        private List<Policy> _subPolicies;

        public CompositePolicy(PolicyOperator op)
        {
            Operator = op;
            op.Level = Level;
            op.ErrorPrefix = ErrorPrefix;
            SubPolicies = new List<Policy>();
        }
        private CompositePolicy() : base()
        {
            _subPolicies = new List<Policy>();
        }

        public override void AddPolicy(Policy policy)
        {
            policy.SetPolicyNode(Level + 1, ErrorPrefix + "\t");
            _subPolicies.Add(policy);
        }
        public override void RemovePolicy(Guid policyId)
        {
            var policy = SubPolicies.FirstOrDefault(p => p.Guid == policyId);
            if (policy != null)
            {
                _subPolicies.Remove(policy);
            }
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
            return _subPolicies.Any() 
                ? Operator.CheckPolicy(user, basket, _subPolicies.ToArray()) 
                : PolicyResult.Ok();
        }

        public override PolicyResult CheckPolicy(StorePurchase purchase)
        {
            return _subPolicies.Any() 
                ? Operator.CheckPolicy(purchase, _subPolicies.ToArray()) 
                : PolicyResult.Ok();
        }
    }
}
