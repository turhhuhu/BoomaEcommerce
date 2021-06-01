using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Domain.Discounts.Operators
{
    public class MaxDiscountOperator : DiscountOperator
    {
        public override string ApplyOperator(Purchase purchase, List<Discount> discounts)
        {
            var bestDiscount = discounts[0];
            var minTotalPrice = decimal.MaxValue;

            foreach (var discount in discounts)
            {
                var totalPrice = discount.CalculateTotalPriceWithoutApplying(purchase);
                if (totalPrice >= minTotalPrice) continue;
                minTotalPrice = totalPrice;
                bestDiscount = discount;
            }

            return "Applied most worthwhile discount:\n" + bestDiscount.ApplyDiscount(purchase);
        }
    }
}
