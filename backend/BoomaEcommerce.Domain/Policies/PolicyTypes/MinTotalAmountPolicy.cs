using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Domain.Policies.PolicyTypes
{
    public class MinTotalAmountPolicy : Policy
    {
        public int MinAmount { get; set; }

        public MinTotalAmountPolicy(int minAmount)
        {
            MinAmount = minAmount;
            ErrorMessage = "Purchase\\Basket must at-least have '{1}' amount but has '{2}' amount.";
        }

        public override PolicyResult CheckPolicy(User user, ShoppingBasket basket)
        {
            var totalAmount = basket.PurchaseProducts
                .Sum(p => p.Amount);

            return totalAmount >= MinAmount
                ? PolicyResult.Ok()
                : PolicyResult.Fail(string.Format(ErrorMessage, MinAmount, totalAmount));
        }

        public override PolicyResult CheckPolicy(StorePurchase purchase)
        {
            var totalAmount = purchase.PurchaseProducts
                .Sum(p => p.Amount);

            return totalAmount >= MinAmount
                ? PolicyResult.Ok()
                : PolicyResult.Fail(string.Format(ErrorMessage, MinAmount, totalAmount));
        }

        private MinTotalAmountPolicy()
        {
            
        }
    }
}
