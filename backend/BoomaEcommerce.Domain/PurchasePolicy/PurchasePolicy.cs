using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Core;

namespace BoomaEcommerce.Domain.PurchasePolicy
{
    public abstract class PurchasePolicy : BaseEntity
    {
        protected PurchasePolicy()
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
}
