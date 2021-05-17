using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Domain.PurchasePolicy.Policies
{
    public class MaxCategoryAmountPolicy : PurchasePolicy
    {
        public string Category { get; set; }
        public int MaxAmount { get; set; }

        public MaxCategoryAmountPolicy(string category, int maxAmount)
        {
            Category = category;
            MaxAmount = maxAmount;
        }
        public override bool CheckPolicy(User user, ShoppingBasket basket)
        {
            return basket.PurchaseProducts
                .Values
                .Where(p => p.Product.Category == Category)
                .Sum(p => p.Amount) <= MaxAmount;
        }

        public override bool CheckPolicy(StorePurchase purchase)
        {
            return purchase
                .PurchaseProducts
                .Where(p => p.Product.Category == Category)
                .Sum(p => p.Amount) <= MaxAmount;
        }
    }
}
