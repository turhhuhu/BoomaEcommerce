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
        private Store _store;

        public BasketDiscount(int percentage, DateTime startTime, DateTime endTime, string description, Policy policy, Store s) :
            base(percentage, startTime, endTime, description, policy)
        {
            _store = s;
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
                
                if (sp.Store.Guid != _store.Guid) continue;
                
                var discountDecimal = (((decimal) 100 - (decimal) Percentage) / (decimal) 100);
                
                foreach (var pp in sp.PurchaseProducts)
                {
                    var productPriceBeforeDiscount = pp.Price;
                    
                    pp.Price *= discountDecimal;
                    
                    discountInfo += "Applied " + Percentage.ToString() + "% discount to " + pp.Product.Name.ToString() + 
                                    " product that belongs to " + _store.StoreName.ToString() + " store\n";

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
                    if (pp.Product.Store.Guid == _store.Guid)
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
