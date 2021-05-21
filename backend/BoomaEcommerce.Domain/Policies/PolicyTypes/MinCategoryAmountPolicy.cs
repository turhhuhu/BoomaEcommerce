using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Domain.Policies.PolicyTypes
{
    public class MinCategoryAmountPolicy : Policy
    {
        public string Category { get; set; }
        public int MinAmount { get; set; }

        public MinCategoryAmountPolicy(string category, int minAmount)
        {
            Category = category;
            MinAmount = minAmount;
            ErrorMessage = "Category '{0}' must at-least have '{1}' amount of products but has '{2}' amount.";
        }
        public override PolicyResult CheckPolicy(User user, ShoppingBasket basket)
        {
            var totalAmount = basket.PurchaseProducts
                .Values
                .Where(p => p.Product.Category == Category)
                .Sum(p => p.Amount);

            return totalAmount >= MinAmount
                ? PolicyResult.Ok()
                : PolicyResult.Fail(string.Format(ErrorMessage, Category, MinAmount, totalAmount));
        }

        public override PolicyResult CheckPolicy(StorePurchase purchase)
        {
            var totalAmount = purchase.PurchaseProducts
                .Where(p => p.Product.Category == Category)
                .Sum(p => p.Amount);

            return totalAmount >= MinAmount
                ? PolicyResult.Ok()
                : PolicyResult.Fail(string.Format(ErrorMessage, Category, MinAmount, totalAmount));
        }
    }
}
