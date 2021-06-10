using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Domain.Discounts.Operators
{
    public class MaxDiscountOperator : DiscountOperator
    {
        public override string ApplyOperator(StorePurchase sp, List<Discount> discounts)
        {
            var bestDiscount = discounts[0];
            var maximalDiscountValue = decimal.MinValue;

            var productsX_updatedPrices = new Dictionary<Guid, decimal>();

            foreach (var discount in discounts)
            {
                foreach (var pp in sp.PurchaseProducts)
                {
                    productsX_updatedPrices[pp.Guid] = pp.Price;
                }
                var calculatedDiscount = discount.CalculateTotalPriceWithoutApplying(sp, productsX_updatedPrices);
                if (calculatedDiscount < maximalDiscountValue) continue;
                maximalDiscountValue = calculatedDiscount;
                bestDiscount = discount;
            }

            return "Applied most worthwhile discount:\n" + bestDiscount.ApplyDiscount(sp);
        }

        public override string ApplyOperator(User user, ShoppingBasket basket, List<Discount> discounts)
        {
            var bestDiscount = discounts[0];
            var maximalDiscountValue = decimal.MinValue;

            foreach (var discount in discounts)
            {
                var calculatedDiscount = discount.CalculateTotalPriceWithoutApplying(user, basket);
                if (calculatedDiscount < maximalDiscountValue) continue;
                maximalDiscountValue = calculatedDiscount;
                bestDiscount = discount;
            }

            return "Applied most worthwhile discount:\n" + bestDiscount.ApplyDiscount(user, basket);
        }
    }
}
