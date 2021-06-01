using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Domain.Policies;

namespace BoomaEcommerce.Domain.Discounts
{
    public class CategoryDiscount : Discount
    {
        private readonly string _category;

        public CategoryDiscount(int percentage, DateTime startTime, DateTime endTime, string description, Policy policy, string category) :
            base(percentage, startTime, endTime, description, policy)
        {
            _category = category;
        }
        public override string ApplyDiscount(Purchase purchase)
        {
            var discountInfo = "";
            decimal totalMoneySaved = 0;

            foreach (var sp in purchase.StorePurchases.Where(sp => !(ValidateDiscount(sp))))
            {
                return "Could not apply discount!\n" + Policy.CheckPolicy(sp).PolicyError;
            }

            foreach (var sp in purchase.StorePurchases)
            {
                decimal storeMoneySaved = 0;
                foreach (var pp in sp.PurchaseProducts)
                {
                    if (!pp.Product.Category.Equals(_category)) continue;
                    
                    var productPriceBeforeDiscount = pp.Price;

                    var discountDecimal = (((decimal)100 - (decimal)Percentage) / (decimal)100);
                    
                    pp.Price *= discountDecimal;

                    discountInfo += "Applied " + Percentage.ToString() + "% discount to " + pp.Product.Name.ToString() +
                                    " of the " + _category + " category\n";

                    storeMoneySaved += (productPriceBeforeDiscount - pp.Price);
                }

                sp.TotalPrice -= storeMoneySaved;
                totalMoneySaved += storeMoneySaved;
            }

            purchase.TotalPrice -= totalMoneySaved;

            return discountInfo;
        }

        public override decimal CalculateTotalPriceWithoutApplying(Purchase purchase)
        {
            decimal totalPrice = 0;

            if (purchase.StorePurchases.Any(sp => !ValidateDiscount(sp)))
            {
                return -1;
            }

            foreach (var sp in purchase.StorePurchases)
            {
                decimal spPrice = 0;
                foreach (var pp in sp.PurchaseProducts)
                {
                    if (pp.Product.Category.Equals(_category))
                    {
                        var discountDecimal = (((decimal)100 - (decimal)Percentage) / (decimal)100);
                        spPrice += (pp.Price * discountDecimal);
                    }
                    else
                    {
                        spPrice += pp.Price;
                    }
                }

                totalPrice += spPrice;
            }

            return totalPrice;
        }
    }
}
