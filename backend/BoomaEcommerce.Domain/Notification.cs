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
        public StorePurchase Purchase { get; }

        public StorePurchaseNotification(
            StorePurchase purchase,
            string message = "A store purchase has been made.") 
            : base(message)
        {
            Purchase = purchase;
        }
    }
}
