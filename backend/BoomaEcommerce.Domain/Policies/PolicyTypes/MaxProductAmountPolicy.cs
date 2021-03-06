using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Domain.Policies.PolicyTypes
{
    public class MaxProductAmountPolicy : ProductPolicy
    {
        public int MaxAmount { get; set; }

        public MaxProductAmountPolicy(Product product, int maxAmount) : base(product)
        {
            Product = product;
            MaxAmount = maxAmount;
            ErrorMessage = "Product '{0}' must at-most have '{1}' amount but has '{2}' amount.";
        }

        private MaxProductAmountPolicy()
        {
            ErrorMessage = "Product '{0}' must at-most have '{1}' amount but has '{2}' amount.";
        }
        public override PolicyResult CheckPolicy(User user, ShoppingBasket basket)
        {
            var purchaseProduct = basket.PurchaseProducts
                .FirstOrDefault(p => p.Product.Guid == Product.Guid);

            return (purchaseProduct?.Amount ?? 0) <= MaxAmount
                ? PolicyResult.Ok()
                : PolicyResult.Fail(string.Format(ErrorMessage, Product.Name, MaxAmount, purchaseProduct?.Amount ?? 0));
        }

        public override PolicyResult CheckPolicy(StorePurchase purchase)
        {
            var purchaseProduct = purchase
                .PurchaseProducts
                .FirstOrDefault(p => p.Product.Guid == Product.Guid);

            return (purchaseProduct?.Amount ?? 0) <= MaxAmount
                ? PolicyResult.Ok()
                : PolicyResult.Fail(string.Format(ErrorMessage, Product.Name, MaxAmount, purchaseProduct?.Amount ?? 0));
        }
    }
}
