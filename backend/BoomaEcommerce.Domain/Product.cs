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
        public double Price { get; set; }
        public int Amount { get; set; }
        public float Rating { get; set; }
        public SemaphoreSlim ProductLock { get; set; }
        public bool IsSoftDeleted { get; set; }
        
        public double CalculatePrice(int amount)
        {
            // TODO: Might need to change to use the product discount type
            // TODO: Might need to check for amount validation
            return Price * amount; 
        }
        public bool PurchaseAmount(int amount)
        {
            if (!ValidateAmount(amount) || IsSoftDeleted)
            {
                return false;
            }

            Amount -= amount;
            return true;
        }

        public bool ValidateAmount(int amount)
        {
            return amount > 0 && amount <= Amount;
        }
    }
}