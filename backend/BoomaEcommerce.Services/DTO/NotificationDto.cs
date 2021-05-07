using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Services.DTO
{
    public class NotificationDto
    {
        public Guid NotifiedUserGuid { get; set; }
        public string Message { get; set; }
        public bool WasSeen { get; set; }
    }
    public class StorePurchaseNotificationDto : NotificationDto
    {
        public StorePurchaseDto Purchases { get; set; }
    }
}
