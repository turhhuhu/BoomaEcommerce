using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Core.Exceptions;
using BoomaEcommerce.Domain.Policies.Operators;

namespace BoomaEcommerce.Domain.Policies
{
    public class BinaryPolicy : MultiPolicy
    {

        private Policy _firstPolicy;
        private Policy _secondPolicy;
        public Policy FirstPolicy
        {
            get => _firstPolicy;
            set
            {
                _firstPolicy = value;
                _firstPolicy?.SetPolicyNode(Level, ErrorPrefix);
            }
        }

        public Policy SecondPolicy
        {
            get => _secondPolicy;
            set
            {
                _secondPolicy = value;
                _secondPolicy?.SetPolicyNode(Level, ErrorPrefix);
            }
        }

        public BinaryPolicy(PolicyOperator @operator, Policy firstPolicy, Policy secondPolicy)
        {
            Operator = @operator;
            FirstPolicy = firstPolicy;
            SecondPolicy = secondPolicy;
        }

        public BinaryPolicy(PolicyOperator @operator)
        {
            Operator = @operator;
            FirstPolicy = Empty;
            SecondPolicy = Empty;
        }
        public override PolicyResult CheckPolicy(User user, ShoppingBasket basket)
        {
            if (FirstPolicy == null || SecondPolicy == null)
            {
                throw new PolicyValidationException(new PolicyError(basket.Guid, "One sub policy of the binary policy is not set."));
            }
            return Operator.CheckPolicy(user, basket, FirstPolicy, SecondPolicy);
        }

        public override PolicyResult CheckPolicy(StorePurchase purchase)
        {
            return Operator.CheckPolicy(purchase, FirstPolicy, SecondPolicy);
        }

        public override void AddPolicy(Policy policy)
        {
            if (FirstPolicy == null)
            {
                FirstPolicy = policy;
            }
            else if (SecondPolicy == null)
            {
                SecondPolicy = policy;
            }
            else
            {
                throw new PolicyValidationException(new PolicyError("Binary policy can consist of only two policies"));
            }
        }

        public override void RemovePolicy(Guid policyGuid)
        {
            if (FirstPolicy?.Guid == policyGuid)
            {
                FirstPolicy = null;
            }
            else if (SecondPolicy?.Guid == policyGuid)
            {
                SecondPolicy = null;
            }
        }

        protected internal override void SetPolicyNode(int level, string prefix)
        {
            base.SetPolicyNode(level, prefix);
            Operator.ErrorPrefix = prefix;
            Operator.Level = level;
            FirstPolicy?.SetPolicyNode(Level + 1, prefix + "\t");
            SecondPolicy?.SetPolicyNode(Level + 1, prefix + "\t");
        }
    }
}
