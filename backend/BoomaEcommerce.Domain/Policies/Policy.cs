using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Core;

namespace BoomaEcommerce.Domain.Policies
{
    public abstract class Policy : BaseEntity
    {

        public static EmptyPolicy Empty => EmptyPolicy.EmptyPol;
        protected Policy()
        {
            Level = 0;
            ErrorMessage = "";
            ErrorPrefix = "";
        }

        protected internal string ErrorMessage { get; set; }
        protected internal string ErrorPrefix { get; set; }
        protected internal int Level { get; set; }

        protected internal virtual void SetPolicyNode(int level, string prefix)
        {

            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ErrorMessage = prefix + ErrorMessage[ErrorPrefix.Length..];
            }
            Level = level;
            ErrorPrefix = prefix;
        }
        public abstract PolicyResult CheckPolicy(User user, ShoppingBasket basket);
        public abstract PolicyResult CheckPolicy(StorePurchase purchase);


    }
    public class EmptyPolicy : Policy
    {
        public static EmptyPolicy EmptyPol => _emptyPolicy ??= new EmptyPolicy();

        private static EmptyPolicy _emptyPolicy;
        private EmptyPolicy()
        {

        }
        public override PolicyResult CheckPolicy(User user, ShoppingBasket basket)
        {
            return PolicyResult.Ok();
        }

        public override PolicyResult CheckPolicy(StorePurchase purchase)
        {
            return PolicyResult.Ok();
        }
    }
}
