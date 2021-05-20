using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Services.DTO
{
    public class NotificationDto
    {
        public virtual string Type { get; set; } = "notification";
        public string Message { get; set; }
        public bool WasSeen { get; set; }
    }
    public class StorePurchaseNotificationDto : NotificationDto
    {
        public override string Type { get; set; } = "storePurchaseNotification";
        public StorePurchaseDto Purchases { get; set; }
    }
}
