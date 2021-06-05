using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Domain.Discounts.Operators
{
    public class SumDiscountOperator : DiscountOperator
    {
        public override string ApplyOperator(StorePurchase sp, List<Discount> discounts)
        {
            List<Discount> orderedDiscounts = discounts.OrderBy(d => d.Percentage).ToList();
            return orderedDiscounts.Aggregate("Combining the following discounts:\n", (current, discount) => current + discount.ApplyDiscount(sp));
        }

        public override string ApplyOperator(User user, ShoppingBasket basket, List<Discount> discounts)
        {
            List<Discount> orderedDiscounts = discounts.OrderBy(d => d.Percentage).ToList();
            return orderedDiscounts.Aggregate("Combining the following discounts:\n", (current, discount) => current + discount.ApplyDiscount(user, basket));
        }
    }
}
