using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Domain.Discounts.Operators;
using BoomaEcommerce.Domain.Policies;

namespace BoomaEcommerce.Domain.Discounts
{
    public class CompositeDiscount : Discount
    {
        public DiscountOperator Operator { get; set; }

        public List<Discount> Discounts;

        public void AddToDiscountList(Discount discount)
        {
            Discounts.Add(discount);
        }

        public CompositeDiscount(int percentage, DateTime startTime, DateTime endTime, string description, Policy policy, DiscountOperator discountOperator) :
            base(percentage, startTime, endTime, description, policy)
        {
            Discounts = new List<Discount>();
            Operator = discountOperator;
        }

        public CompositeDiscount(DiscountOperator discountOperator) : base(0, DateTime.MinValue, DateTime.MaxValue, "", Policy.Empty)
        {
            Discounts = new List<Discount>();
            Operator = discountOperator;
        }

        private CompositeDiscount() : base()
        {
            Discounts = new List<Discount>();
        }

        public override string ApplyDiscount(StorePurchase sp)
        {
            return Discounts.Any() 
                ? Operator.ApplyOperator(sp, Discounts) 
                : string.Empty;
        }

        public override string ApplyDiscount(User user, ShoppingBasket basket)
        {
            return Operator.ApplyOperator(user, basket, Discounts);
        }

        public override decimal CalculateTotalPriceWithoutApplying(StorePurchase sp)
        {
            throw new NotImplementedException();
        }

        public override decimal CalculateTotalPriceWithoutApplying(User user, ShoppingBasket basket)
        {
            throw new NotImplementedException();
        }
    }

}
