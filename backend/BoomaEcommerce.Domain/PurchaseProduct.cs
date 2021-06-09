using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Core;

namespace BoomaEcommerce.Domain
{
    public class PurchaseProduct : BaseEntity
    {
        public Product Product { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountedPrice { get; set; }

        public PurchaseProduct(Product product, int amount, decimal price, decimal discountedPrice)
        {
            Product = product;
            Amount = amount;
            Price = price;
            DiscountedPrice = discountedPrice;
        }
        
        public PurchaseProduct(Product product, int amount, decimal price)
        {
            Product = product;
            Amount = amount;
            Price = price;
        }


        public PurchaseProduct()
        {
            
        }

        public async Task<decimal> CalculatePriceAsync()
        {
            await Product.ProductLock.WaitAsync();
            var price = Product.CalculatePrice(Amount);
            Product.ProductLock.Release();
            return price;
        }
        
        public bool Purchase()
        {
            return Product.PurchaseAmount(Amount);
        }

        public bool ValidatePurchase()
        {
            return Product.ValidateAmountToPurchase(Amount);
        }

        public bool ValidatePrice()
        {
            return Price == Product.CalculatePrice(Amount);
        }
        
    }
}
