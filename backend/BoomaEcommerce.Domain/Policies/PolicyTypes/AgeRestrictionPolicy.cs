using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Domain.Policies.PolicyTypes
{
    public class AgeRestrictionPolicy : ProductPolicy
    {
        public int MinAge { get; set; }

        public AgeRestrictionPolicy(Product product, int minAge) : base(product)
        {
            MinAge = minAge;
            ErrorMessage = "User '{0}' must at-least be of age {1} to purchase {2}.";
        }
        private AgeRestrictionPolicy()
        {
            ErrorMessage = "User '{0}' must at-least be of age {1} to purchase {2}.";
        }
        public override PolicyResult CheckPolicy(User user, ShoppingBasket basket)
        {
            return DateTime.Today.Year - user.DateOfBirth.Year >= MinAge 
                ? PolicyResult.Ok() 
                : PolicyResult.Fail(string.Format(ErrorMessage, user.UserName, MinAge, Product.Name));
        }


        public override PolicyResult CheckPolicy(StorePurchase purchase)
        {
            return DateTime.Today.Year - purchase.Buyer.DateOfBirth.Year >= MinAge
                ? PolicyResult.Ok()
                : PolicyResult.Fail(string.Format(ErrorMessage, purchase.Buyer.UserName, MinAge, Product.Name));
        }
    }
}
