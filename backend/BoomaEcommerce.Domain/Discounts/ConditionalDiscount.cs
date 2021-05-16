using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Domain.Discounts
{
    public class ConditionalDiscount : Discount
    {
        public ConditionalDiscount(int percentage, DateTime startTime, DateTime endTime) : base(percentage, startTime, endTime)
        {
        }
    }
}
