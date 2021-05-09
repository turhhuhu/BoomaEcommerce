using BoomaEcommerce.Services.DTO;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Services
{
    class NotificationCreationConverter : JsonCreationConverter<NotificationDto>
    {
        protected override NotificationDto Create(Type objectType, JObject jObject)
        {
            var type = jObject["type"].ToObject<string>();
            switch(type)
            {
                case "notification":
                    return new NotificationDto();
                case "storePurchaseNotification":
                    return new StorePurchaseNotificationDto();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
