using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Domain.Policies.PolicyTypes
{
    public class MinProductAmountPolicy : ProductPolicy
    {
        public int MinAmount { get; set; }

        public MinProductAmountPolicy(Product product, int minAmount) : base(product)
        {
            Product = product;
            MinAmount = minAmount;
            ErrorMessage = "Product '{0}' must at-least have '{1}' amount but has '{2}' amount.";
        }

        private MinProductAmountPolicy()
        {
            
        }

        public override PolicyResult CheckPolicy(User user, ShoppingBasket basket)
        {
            var purchaseProduct = basket.PurchaseProducts
                .FirstOrDefault(p => p.Product.Guid == Product.Guid);

            return purchaseProduct?.Amount >= MinAmount
                ? PolicyResult.Ok()
                : PolicyResult.Fail(string.Format(ErrorMessage, Product.Name, MinAmount, purchaseProduct?.Amount ?? 0));
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
