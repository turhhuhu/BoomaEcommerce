using BoomaEcommerce.Domain.Policies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Domain.Discounts.Operators;

namespace BoomaEcommerce.Domain.Discounts
{
    public abstract class Discount : BaseEntity
    {
        public int Percentage { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Policy Policy { get; set; }
        public string Description { get; set; }

        protected Discount(int percentage, DateTime startTime, DateTime endTime, string description, Policy policy)
        {
            Percentage = percentage;
            StartTime = startTime;
            EndTime = endTime;
            Description = description;
            Policy = policy;
        }

        protected Discount(int percentage, DateTime startTime, DateTime endTime, string description)
         : this(percentage, startTime, endTime, description, Policy.Empty)
        {
            
        }

        protected bool ValidateDiscount(StorePurchase sp)
        {
            DateTime current = DateTime.Now;
            if (DateTime.Compare(current, StartTime) < 0 || DateTime.Compare(current, EndTime) > 0)
                return false;
            return Policy.CheckPolicy(sp).IsOk;
        }

        protected bool ValidateDiscount(User user, ShoppingBasket basket)
        {
            DateTime current = DateTime.Now;
            if (DateTime.Compare(current, StartTime) < 0 || DateTime.Compare(current, EndTime) > 0)
                return false;
            return Policy.CheckPolicy(user, basket).IsOk;
        }

        // Apply discount at purchase stage 
        public abstract string ApplyDiscount(StorePurchase sp);

        // Apply discount at adding to cart stage 
        public abstract string ApplyDiscount(User user, ShoppingBasket basket);

        // MaxDiscountOperator 
        public abstract decimal CalculateTotalPriceWithoutApplying(StorePurchase sp);
        public abstract decimal CalculateTotalPriceWithoutApplying(User user, ShoppingBasket basket);
    }

    public class EmptyDiscount : Discount
    {
        public EmptyDiscount(int percentage, DateTime startTime, DateTime endTime, string description, Policy policy) : base(percentage, startTime, endTime, description, policy)
        {
        }

        public EmptyDiscount(int percentage, DateTime startTime, DateTime endTime, string description) : base(percentage, startTime, endTime, description)
        {
            this.Policy = Policy.Empty;
        }

        public override string ApplyDiscount(StorePurchase sp)
        {
            return "";
        }

        public override string ApplyDiscount(User user, ShoppingBasket basket)
        {
            return "";
        }

        public override decimal CalculateTotalPriceWithoutApplying(StorePurchase sp)
        {
            return sp.TotalPrice;
        }

        public override decimal CalculateTotalPriceWithoutApplying(User user, ShoppingBasket basket)
        {
            return basket.PurchaseProducts.Sum(pp => pp.Price);
        }
    }
}
