using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Domain.Policies;

namespace BoomaEcommerce.Domain
{
    public class Store : BaseEntity
    {
        public string StoreName { get; set; }
        public string Description { get; set; }
        public User StoreFounder { get; set; }
        public float Rating { get; set; }
        public Policy StorePolicy { get; set; }

        public Store(Policy storePolicy)
        {
            this.StorePolicy = storePolicy;
        }

        public Store()
        {
            this.StorePolicy = Policy.Empty;
        }
        
        public PolicyResult CheckPolicy(StorePurchase purchase)
        {
            return StorePolicy.CheckPolicy(purchase);
        }
        public PolicyResult CheckPolicy(User user, ShoppingBasket basket)
        {
            return StorePolicy.CheckPolicy(user, basket);
        }
    }

}
