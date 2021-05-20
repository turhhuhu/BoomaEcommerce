using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Domain.Policies.Operators;

namespace BoomaEcommerce.Domain.Policies
{
    public class BinaryPolicy : Policy
    {
        public PolicyOperator Operator { get; set; }

        private Policy _firstPolicy;
        private Policy _secondPolicy;
        public Policy FirstPolicy
        {
            get => _firstPolicy;
            set
            {
                _firstPolicy = value;
                _firstPolicy.SetPolicyNode(Level, ErrorPrefix);
            }
        }

        public Policy SecondPolicy
        {
            get => _secondPolicy;
            set
            {
                _secondPolicy = value;
                _secondPolicy.SetPolicyNode(Level, ErrorPrefix);
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
        }

        public override PolicyResult CheckPolicy(User user, ShoppingBasket basket)
        {
            return Operator.CheckPolicy(user, basket, FirstPolicy, SecondPolicy);
        }

        public override PolicyResult CheckPolicy(StorePurchase purchase)
        {
            return Operator.CheckPolicy(purchase, FirstPolicy, SecondPolicy);
        }
        protected internal override void SetPolicyNode(int level, string prefix)
        {
            base.SetPolicyNode(level, prefix);
            Operator.ErrorPrefix = prefix;
            Operator.Level = level;
            FirstPolicy.SetPolicyNode(Level + 1, prefix + "\t");
            SecondPolicy.SetPolicyNode(Level + 1, prefix + "\t");
        }
    }
}
