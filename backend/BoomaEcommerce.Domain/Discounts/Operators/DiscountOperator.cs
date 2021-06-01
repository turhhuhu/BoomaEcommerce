using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BoomaEcommerce.Domain.Discounts.Operators
{
    public abstract class DiscountOperator
    {
        protected DiscountOperator() { }
        public abstract string ApplyOperator(Purchase purchase, List<Discount> discounts);
    }
}
