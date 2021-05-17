using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Domain.PurchasePolicy.Policies
{
    public class AgeRestrictionPolicy : PurchasePolicy
    {
        public Product Product { get; set; }
        public int MinAge { get; set; }
        public AgeRestrictionPolicy(Product product, int minAge)
        {
            Product = product;
            MinAge = minAge;
        }

        public override bool CheckPolicy(User user, ShoppingBasket basket)
        {
            return DateTime.Today.Year - user.DateOfBirth.Year >= MinAge;
        }

        public override bool CheckPolicy(StorePurchase purchase)
        {
            return DateTime.Today.Year - purchase.Buyer.DateOfBirth.Year >= MinAge;
        }
    }
}
