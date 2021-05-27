using BoomaEcommerce.Domain.Policies;
using System;
using System.Collections.Generic;
using System.Linq;
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

        protected Discount(int percentage, DateTime startTime, DateTime endTime)
        {
            Percentage = percentage;
            StartTime = startTime;
            EndTime = endTime;
        }

        public int CalculatePrice()
        {
            throw new NotImplementedException();
        }
    }
}
