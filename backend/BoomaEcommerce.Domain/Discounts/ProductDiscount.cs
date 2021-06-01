using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Domain.Policies;

namespace BoomaEcommerce.Domain.Discounts
{
    public class ProductDiscount : Discount
    {
        private Product _product { get; set; }


        public ProductDiscount(int percentage, DateTime startTime, DateTime endTime, string description, Policy policy, Product product) :
            base(percentage, startTime, endTime, description, policy)
        {
            _product = product;
        }

        public override string ApplyDiscount(Purchase purchase)
        {
            var discountInfo = "";
            decimal totalMoneySaved = 0;


            foreach (var sp in purchase.StorePurchases.Where(sp => !(ValidateDiscount(sp))))
            {
                return "Could not apply discount!\n" + (Policy.CheckPolicy(sp).IsOk ? "Discount validity has expired\n" : Policy.CheckPolicy(sp).PolicyError);
            }

            foreach (var sp in purchase.StorePurchases)
            {
                decimal storeMoneySaved = 0;
                foreach (var pp in sp.PurchaseProducts)
                {
                    if (!(pp.Product.Guid == _product.Guid)) continue;

                    var productPriceBeforeDiscount = pp.Price;

                    var discountDecimal = (((decimal)100 - (decimal)Percentage) / (decimal)100);

                    pp.Price *= discountDecimal;

                    discountInfo += "Applied " + Percentage.ToString() + "% discount to " + _product.Name.ToString() + " product \n";

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
                    if (pp.Product.Guid == _product.Guid)
                    {
                        var discountDecimal = (((decimal) 100 - (decimal) Percentage) / (decimal) 100);
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
