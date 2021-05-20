using BoomaEcommerce.Services.DTO;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Api
{
    class NotificationCreationConverter : JsonCreationConverter<NotificationDto>
    {
        protected override NotificationDto Create(Type objectType, JObject jObject)
        {
            var type = jObject["type"].ToObject<string>();
            return type switch
            {
                "notification" => new NotificationDto(),
                "storePurchaseNotification" => new StorePurchaseNotificationDto(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
