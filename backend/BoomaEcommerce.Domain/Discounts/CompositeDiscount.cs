using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Domain.Discounts.Operators;
using BoomaEcommerce.Domain.Policies;

namespace BoomaEcommerce.Domain.Discounts
{
    public class CompositeDiscount 
    {
        private DiscountOperator _operator { get; set; }

        private readonly List<Discount> _discounts;

        public CompositeDiscount(DiscountOperator discountOperator)
        {
            _discounts = new List<Discount>();
            _operator = discountOperator;
        }

        public string ApplyDiscount(Purchase purchase)
        {
            return _operator.ApplyOperator(purchase, _discounts);
        }

        public decimal CalculateTotalPriceWithoutApplying(Purchase purchase)
        {
            throw new NotImplementedException();
        }

        public void AddToDiscountList(Discount discount)
        {
            _discounts.Add(discount);
        }
    }
}
