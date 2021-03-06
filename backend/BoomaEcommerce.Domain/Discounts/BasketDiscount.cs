using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Domain.Policies;

namespace BoomaEcommerce.Domain.Discounts
{
    public class BasketDiscount : Discount
    {
        public BasketDiscount(int percentage, DateTime startTime, DateTime endTime, string description, Policy policy) :
            base(percentage, startTime, endTime, description, policy)
        {
        }

        private BasketDiscount() : base(0, DateTime.MinValue, DateTime.MaxValue, "", Policy.Empty)
        {
            
        }
        public override string ApplyDiscount(StorePurchase sp)
        {
            var discountInfo = "";
            decimal moneySaved = 0;

            if (!(ValidateDiscount(sp)))
            {
                return "Could not apply discount!\n" + (Policy.CheckPolicy(sp).IsOk ? "Discount validity has expired\n" : Policy.CheckPolicy(sp).PolicyError);
            }

            foreach (var pp in sp.PurchaseProducts.Where(pp => pp.Product.Store.Guid == sp.Store.Guid))
            {
                var productPriceBeforeDiscount = pp.DiscountedPrice;

                var discountDecimal = ((100 - (decimal)Percentage) / 100);

                pp.DiscountedPrice *= discountDecimal;

                discountInfo += "Applied " + Percentage + "% discount to " + pp.Product.Name +
                                " product that belongs to " + sp.Store.StoreName + " store\n";

                moneySaved += productPriceBeforeDiscount - pp.DiscountedPrice;
            }

            sp.DiscountedPrice -= moneySaved;

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

            foreach (var pp in basket.PurchaseProducts)
            {

                if (!(pp.Product.Guid == basket.Store.Guid)) continue;

                var discountDecimal = ((100 - (decimal)Percentage) / 100);

                pp.DiscountedPrice = pp.Price * discountDecimal;

                discountInfo += "Applied " + Percentage + "% discount to " + pp.Product.Name +
                                " product that belongs to " + basket.Store.StoreName + " store\n";
            }

            return discountInfo;
        }

        public override decimal CalculateTotalPriceWithoutApplying(StorePurchase sp, Dictionary<Guid, decimal> updatedPrices)
        {
            decimal calculatedDiscount = 0;
            decimal ppDiscount = 0;

            if (!ValidateDiscount(sp))
            {
                return decimal.MaxValue;
            }


            foreach (var pp in sp.PurchaseProducts.Where(pp => pp.Product.Store.Guid == sp.Store.Guid))
            {
                var productPriceBeforeDiscount = updatedPrices[pp.Guid];

                var discountDecimal = ((100 - (decimal)Percentage) / 100);

                ppDiscount = (productPriceBeforeDiscount - productPriceBeforeDiscount * discountDecimal);

                updatedPrices[pp.Guid] -= ppDiscount;

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


            foreach (var pp in basket.PurchaseProducts)
            {

                if (pp.Product.Guid == basket.Store.Guid)
                {
                    ppDiscount = pp.Price - (pp.Price * ((100 - (decimal)Percentage) / 100));
                }

                calculatedDiscount += ppDiscount;
            }

            return calculatedDiscount;
        }
    }
}
