using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Core;

namespace BoomaEcommerce.Domain
{
    public class Notification : BaseEntity
    {
        public string Message { get; set; }
        public bool WasSeen { get; set; }

        public Notification(string message)
        {
            Message = message;
            WasSeen = false;
        }
    }

    public class StorePurchaseNotification : Notification
    {
        public User Buyer { get; set; }
        public Store Store { get; set; }
        public Guid StorePurchaseGuid { get; set; }

        public StorePurchaseNotification(User buyer, Guid storePurchaseGuid, Store store, string message = "A store purchase has been made.") 
            : base(message)
        {
            Buyer = buyer;
            StorePurchaseGuid = storePurchaseGuid;
            Store = store;
        }
    }
    public class RoleDismissalNotification : Notification
    {
        public User DismissingUser { get; set; }
        public Store Store { get; set; }


        public RoleDismissalNotification(User dismissingUser, Store store,
            string message = "You've been dismissed from an ownership store role.") : base(message)
        {
            DismissingUser = dismissingUser;
            Store = store;
        }
    }
}
