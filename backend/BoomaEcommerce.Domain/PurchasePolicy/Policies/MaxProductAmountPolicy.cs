using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Domain.PurchasePolicy.Policies
{
    public class MaxProductAmountPolicy : PurchasePolicy
    {
        public Product Product { get; set; }
        private int MaxAmount { get; set; }

        public MaxProductAmountPolicy(Product product, int maxAmount)
        {
            Product = product;
            MaxAmount = maxAmount;
        }

        public override bool CheckPolicy(User user, ShoppingBasket basket)
        {
            var purchaseProduct = basket.PurchaseProducts
                .Values
                .FirstOrDefault(p => p.Product.Guid == Product.Guid);

            return purchaseProduct == null || purchaseProduct.Amount <= MaxAmount;
        }

        public override bool CheckPolicy(StorePurchase purchase)
        {
            var purchaseProduct = purchase
                .PurchaseProducts
                .FirstOrDefault(p => p.Product.Guid == Product.Guid);

            return purchaseProduct == null || purchaseProduct.Amount <= MaxAmount;
        }
    }
}
