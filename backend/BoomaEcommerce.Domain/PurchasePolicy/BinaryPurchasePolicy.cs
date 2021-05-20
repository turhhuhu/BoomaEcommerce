using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Domain.PurchasePolicy.Operators;

namespace BoomaEcommerce.Domain.PurchasePolicy
{
    public class BinaryPurchasePolicy : PurchasePolicy
    {
        public PolicyOperator Operator { get; set; }

        private PurchasePolicy _firstPolicy;
        private PurchasePolicy _secondPolicy;
        public PurchasePolicy FirstPolicy
        {
            get => _firstPolicy;
            set
            {
                _firstPolicy = value;
                _firstPolicy.SetPolicyNode(Level, ErrorPrefix);
            }
        }

        public PurchasePolicy SecondPolicy
        {
            get => _secondPolicy;
            set
            {
                _secondPolicy = value;
                _secondPolicy.SetPolicyNode(Level, ErrorPrefix);
            }
        }

        public BinaryPurchasePolicy(PolicyOperator @operator, PurchasePolicy firstPolicy, PurchasePolicy secondPolicy)
        {
            Operator = @operator;
            FirstPolicy = firstPolicy;
            SecondPolicy = secondPolicy;
        }

        public BinaryPurchasePolicy(PolicyOperator @operator)
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
