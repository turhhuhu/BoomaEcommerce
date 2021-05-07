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
        public User NotifiedUser { get; set; }
        public string Message { get; set; }
        public bool WasSeen { get; set; }

        protected Notification(User notifiedUser, string message)
        {
            NotifiedUser = notifiedUser;
            Message = message;
            WasSeen = false;
        }
    }

    public class StorePurchaseNotification : Notification
    {
        public StorePurchase Purchase { get; }

        public StorePurchaseNotification(User notifiedUser,
            StorePurchase purchase,
            string message = "A store purchase has been made.") 
            : base(notifiedUser, message)
        {
            Purchase = purchase;
        }
    }
}
