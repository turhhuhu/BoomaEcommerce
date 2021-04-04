using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Core;

namespace BoomaEcommerce.Domain
{
    public class StorePurchase : BaseEntity
    {
        public List<PurchaseProduct> ProductsPurchases { get; set; }
        public User Buyer { get; set; }
        public Store Store { get; set; }
        public double TotalPrice { get; set; }
    }
}
