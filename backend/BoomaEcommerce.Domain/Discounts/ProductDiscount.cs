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
        public Product Product { get; set; }

        public ProductDiscount(int percentage, DateTime startTime, DateTime endTime, string description, Policy policy, Product product) :
            base(percentage, startTime, endTime, description, policy)
        {
            Product = product;
        }

        public ProductDiscount(Product product) :
            base(0, DateTime.MinValue, DateTime.MaxValue, "", Policy.Empty)
        {
            Product = product;
        }

        public override string ApplyDiscount(StorePurchase sp)
        {
            var discountInfo = "";
            decimal moneySaved = 0;

            if(!(ValidateDiscount(sp)))
            {
                return "Could not apply discount!\n" + (Policy.CheckPolicy(sp).IsOk ? "Discount validity has expired\n" : Policy.CheckPolicy(sp).PolicyError);
            }

            foreach (var pp in sp.PurchaseProducts)
            {
                if (!(pp.Product.Guid == Product.Guid)) continue;

                var productPriceBeforeDiscount = pp.Price;

                var discountDecimal = (((decimal)100 - (decimal)Percentage) / (decimal)100);

                pp.Price *= discountDecimal;

                discountInfo += "Applied " + Percentage.ToString() + "% discount to " + pp.Product.Name.ToString() + " product \n";

                moneySaved += (productPriceBeforeDiscount - pp.Price);
            }

            sp.DiscountedPrice = sp.DiscountedPrice - moneySaved;

            return discountInfo;
        }

        public override string ApplyDiscount(User user, ShoppingBasket basket)
        {
            var discountInfo = "";

            if (!(ValidateDiscount(user, basket)))
            {
                return "Could not apply discount!\n" + (Policy.CheckPolicy(user, basket).IsOk ? 
                    "Discount validity has expired\n" : Policy.CheckPolicy(user, basket).PolicyError);
            }

            foreach (var pair in basket.PurchaseProducts)
            {
                var pp = pair.Value;

                if (!(pp.Product.Guid == Product.Guid)) continue;

                var discountDecimal = (((decimal)100 - (decimal)Percentage) / (decimal)100);

                pp.DiscountedPrice = pp.Price * discountDecimal;

                discountInfo += "Applied " + Percentage.ToString() + "% discount to " + Product.Name.ToString() + " product \n";
            }

            return discountInfo;
        }

        public override decimal CalculateTotalPriceWithoutApplying(StorePurchase sp)
        {
            decimal calculatedDiscount = 0;
            decimal ppDiscount = 0;

            if (!ValidateDiscount(sp))
            {
                return decimal.MaxValue;
            }

            
            foreach (var pp in sp.PurchaseProducts)
            {
                if (pp.Product.Guid == Product.Guid)
                {
                    ppDiscount = pp.Price - (pp.Price * (((decimal) 100 - (decimal) Percentage) / (decimal) 100));
                }

                calculatedDiscount += ppDiscount;
            }

            return calculatedDiscount;
        }

        public override decimal CalculateTotalPriceWithoutApplying(User user, ShoppingBasket basket)
        {
            decimal calculatedDiscount = 0;
            decimal ppDiscount = 0;

            if (!ValidateDiscount(user, basket))
            {
                return decimal.MaxValue;
            }


            foreach (var pair in basket.PurchaseProducts)
            {
                var pp = pair.Value;

                if (pp.Product.Guid == Product.Guid)
                {
                    ppDiscount = pp.Price - (pp.Price * (((decimal)100 - (decimal)Percentage) / (decimal)100));
                }

                calculatedDiscount += ppDiscount;
            }

            return calculatedDiscount;
        }
    }
}
