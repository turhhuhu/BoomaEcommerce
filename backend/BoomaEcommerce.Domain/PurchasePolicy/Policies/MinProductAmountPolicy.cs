using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Domain.PurchasePolicy.Policies
{
    public class MinProductAmountPolicy : PurchasePolicy
    {
        public Product Product { get; set; }
        private int MinAmount { get; set; }

        public MinProductAmountPolicy(Product product, int minAmount)
        {
            Product = product;
            MinAmount = minAmount;
        }

        public override bool CheckPolicy(User user, ShoppingBasket basket)
        {
            var purchaseProduct = basket.PurchaseProducts
                .Values
                .FirstOrDefault(p => p.Product.Guid == Product.Guid);

            return purchaseProduct == null || purchaseProduct.Amount >= MinAmount;
        }

        public override bool CheckPolicy(StorePurchase purchase)
        {
            var purchaseProduct = purchase
                .PurchaseProducts
                .FirstOrDefault(p => p.Product.Guid == Product.Guid);

            return purchaseProduct == null || purchaseProduct.Amount >= MinAmount;
        }
    }
}
