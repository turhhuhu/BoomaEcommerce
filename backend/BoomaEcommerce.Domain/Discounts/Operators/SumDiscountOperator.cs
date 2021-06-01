using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Domain.Discounts.Operators
{
    public class SumDiscountOperator : DiscountOperator
    {
        public override string ApplyOperator(Purchase purchase, List<Discount> discounts)
        {
            List<Discount> orderedDiscounts = discounts.OrderBy(d => d.Percentage).ToList();
            return orderedDiscounts.Aggregate("Combining the following discounts:\n", (current, discount) => current + discount.ApplyDiscount(purchase));
        }
    }
}
