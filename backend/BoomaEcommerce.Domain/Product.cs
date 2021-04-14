using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BoomaEcommerce.Core;

namespace BoomaEcommerce.Domain
{
    public class Product : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Store Store { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public int Amount { get; set; }
        public float Rating { get; set; }
        public SemaphoreSlim ProductLock { get; set; }
        public bool IsSoftDeleted { get; set; }
        public IPurchaseType PurchaseType { get; set; }
        
        public decimal CalculatePrice(int amount)
        {
            // TODO: Might need to change to use the product discount type
            return Price * amount; 
        }
        public bool PurchaseAmount(int amount)
        {
            if (!ValidateAmountToPurchase(amount) || IsSoftDeleted)
            {
                return false;
            }

            Amount -= amount;
            return true;
        }

        public bool ValidateAmountToPurchase(int amount)
        {
            return amount > 0 && amount <= Amount;
        }

        public bool ValidateAmount()
        {
            return Amount >= 0;
        }

        public bool ValidateStorePolicy()
        {
            return Store.StorePolicy.CheckPolicy(PurchaseType);
        }
    }
}