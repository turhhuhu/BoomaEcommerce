using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Domain.Policies.PolicyTypes
{
    public class MaxTotalAmountPolicy : Policy
    {
        public int MaxAmount { get; set; }

        public MaxTotalAmountPolicy(int maxAmount)
        {
            MaxAmount = maxAmount;
            ErrorMessage = "Purchase\\Basket must at-most have '{0}' amount but has '{1}' amount.";
        }

        private MaxTotalAmountPolicy()
        {
            ErrorMessage = "Purchase\\Basket must at-most have '{0}' amount but has '{1}' amount.";
        }
        public override PolicyResult CheckPolicy(User user, ShoppingBasket basket)
        {
            var totalAmount = basket.PurchaseProducts
                .Sum(p => p.Amount);

            return totalAmount <= MaxAmount
                ? PolicyResult.Ok()
                : PolicyResult.Fail(string.Format(ErrorMessage, MaxAmount, totalAmount));
        }

        public override PolicyResult CheckPolicy(StorePurchase purchase)
        {
            var totalAmount = purchase.PurchaseProducts
                .Sum(p => p.Amount);

            return totalAmount <= MaxAmount
                ? PolicyResult.Ok()
                : PolicyResult.Fail(string.Format(ErrorMessage, MaxAmount, totalAmount));
        }
    }
}
