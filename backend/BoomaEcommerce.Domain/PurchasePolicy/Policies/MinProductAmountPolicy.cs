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
            ErrorMessage = "Product '{0}' must at-least have '{1}' amount but has '{2}' amount.";
        }

        public override PolicyResult CheckPolicy(User user, ShoppingBasket basket)
        {
            var purchaseProduct = basket.PurchaseProducts
                .Values
                .FirstOrDefault(p => p.Product.Guid == Product.Guid);

            if(purchaseProduct == null || purchaseProduct.Amount >= MinAmount)
            {
                return PolicyResult.Ok();
            }
            return PolicyResult.Fail(string.Format(ErrorMessage, Product.Name, MinAmount, purchaseProduct.Amount));
        }

        public override PolicyResult CheckPolicy(StorePurchase purchase)
        {
            var purchaseProduct = purchase
                .PurchaseProducts
                .FirstOrDefault(p => p.Product.Guid == Product.Guid);

            return purchaseProduct?.Amount >= MinAmount 
                ? PolicyResult.Ok() 
                : PolicyResult.Fail(string.Format(ErrorMessage, Product.Name, MinAmount, purchaseProduct?.Amount ?? 0));
        }
    }
}
