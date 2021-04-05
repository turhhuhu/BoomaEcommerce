using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Core;

namespace BoomaEcommerce.Domain
{
    public class ShoppingBasket : BaseEntity
    {
        public Store Store { get; set; }
        public List<PurchaseProduct> PurchaseProducts { get; set; }

    }
}
