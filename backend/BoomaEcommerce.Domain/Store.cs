using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Domain.Discounts;
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
        public Discount StoreDiscount { get; set; }

        public Store(Policy storePolicy)
        {
            this.StorePolicy = storePolicy;
        }

        public Store()
        {
            this.StorePolicy = Policy.Empty;
            this.StoreDiscount = Discount.Empty;
        }
        
        public PolicyResult CheckPolicy(StorePurchase purchase)
        {
            return StorePolicy.CheckPolicy(purchase);
        }
        public PolicyResult CheckPolicy(User user, ShoppingBasket basket)
        {
            return StorePolicy.CheckPolicy(user, basket);
        }

        public string ApplyDiscount(StorePurchase sp)
        {
            return StoreDiscount.ApplyDiscount(sp);
        }

        public string ApplyDiscount(User user, ShoppingBasket basket)
        {
            return StoreDiscount.ApplyDiscount(user, basket);
        }

    }

}
