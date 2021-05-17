﻿using System;
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
            ErrorMessage = "Product '{0}' must at-most have '{1}' amount but has '{2}' amount.";
        }

        public override PolicyResult CheckPolicy(User user, ShoppingBasket basket)
        {
            var purchaseProduct = basket.PurchaseProducts
                .Values
                .FirstOrDefault(p => p.Product.Guid == Product.Guid);

            if (purchaseProduct == null || purchaseProduct.Amount <= MaxAmount)
            {
                return PolicyResult.Ok();
            }
            return PolicyResult.Fail(string.Format(ErrorMessage, Product.Name, MaxAmount, purchaseProduct.Amount));
        }

        public override PolicyResult CheckPolicy(StorePurchase purchase)
        {
            var purchaseProduct = purchase
                .PurchaseProducts
                .FirstOrDefault(p => p.Product.Guid == Product.Guid);

            if (purchaseProduct == null || purchaseProduct.Amount <= MaxAmount)
            {
                return PolicyResult.Ok();
            }
            return PolicyResult.Fail(string.Format(ErrorMessage, Product.Name, MaxAmount, purchaseProduct.Amount));
        }
    }
}
