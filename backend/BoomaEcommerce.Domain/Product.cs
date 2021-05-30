using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BoomaEcommerce.Core;

namespace BoomaEcommerce.Domain
{
    public class Product : BaseEntity
    {
        public const int MaxRating = 10;
        public const int MinRating = 0;

        public int Id { get; set; }
        public string Name { get; set; }

        public Store Store { get; set; }
        public string Category { get; set; }


        public decimal Price
        {
            get => _price;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(Price));
                }

                _price = value;
            }
        }
        public int Amount { get; set; }
        public decimal Rating
        {
            get => _rating;
            set
            {
                if (value > MaxRating || value < MinRating)
                {
                    throw new ArgumentOutOfRangeException(nameof(Rating));
                }

                _rating = value;
            }
        }
        public SemaphoreSlim ProductLock { get; set; } = new(1);
        public bool IsSoftDeleted { get; set; }

        private decimal _price;
        private decimal _rating;
        
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

    }
}