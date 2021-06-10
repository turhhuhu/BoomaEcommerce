using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using Microsoft.Extensions.Logging;

namespace BoomaEcommerce.Domain.Discounts.Operators
{
    public abstract class DiscountOperator : BaseEntity
    {
        protected DiscountOperator() { }
        public abstract string ApplyOperator(StorePurchase sp, List<Discount> discounts);
        public abstract string ApplyOperator(User user, ShoppingBasket basket, List<Discount> discounts);
    }
}
