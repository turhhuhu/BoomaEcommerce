using BoomaEcommerce.Domain.Policies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Domain.Discounts
{
    public abstract class Discount : IDiscountType
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

        protected bool ValidateDiscount(StorePurchase sp)
        {
            DateTime current = DateTime.Now;
            if (DateTime.Compare(current, StartTime) < 0 || DateTime.Compare(current, EndTime) > 0)
                return false;
            return Policy.CheckPolicy(sp).IsOk;
        }

        public abstract string ApplyDiscount(Purchase purchase);

        // MaxDiscountOperator 
        public abstract decimal CalculateTotalPriceWithoutApplying(Purchase purchase);
    }
}
