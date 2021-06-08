using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Domain.Policies.PolicyTypes
{
    public class MaxCategoryAmountPolicy : Policy
    {
        public string Category { get; set; }
        public int MaxAmount { get; set; }

        public MaxCategoryAmountPolicy(string category, int maxAmount)
        {
            Category = category;
            MaxAmount = maxAmount;
            ErrorMessage = "Category '{0}' must at-most have '{1}' amount of products but has '{2}' amount.";
        }

        private MaxCategoryAmountPolicy()
        {
            
        }

        public override PolicyResult CheckPolicy(User user, ShoppingBasket basket)
        {
            var totalAmount = basket.PurchaseProducts
                .Where(p => p.Product.Category == Category)
                .Sum(p => p.Amount);

            return totalAmount <= MaxAmount 
                ? PolicyResult.Ok() 
                : PolicyResult.Fail(string.Format(ErrorMessage, Category, MaxAmount, totalAmount));
        }

        public override PolicyResult CheckPolicy(StorePurchase purchase)
        {
            var totalAmount =  purchase
                .PurchaseProducts
                .Where(p => p.Product.Category == Category)
                .Sum(p => p.Amount);

            return totalAmount <= MaxAmount
                ? PolicyResult.Ok()
                : PolicyResult.Fail(string.Format(ErrorMessage, Category, MaxAmount, totalAmount));
        }
    }
}
