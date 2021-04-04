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

        public PurchaseProduct(Product product, int amount)
        {
            Product = product;
            Amount = amount;
        }

        public async Task<double> CalculatePriceAsync()
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
            return Product.ValidateAmount(Amount);
        }
    }
}
