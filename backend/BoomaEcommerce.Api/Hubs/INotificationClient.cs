using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Services.DTO;

namespace BoomaEcommerce.Api.Hubs
{
    public interface INotificationClient
    {
        Task ReceiveNotification(NotificationDto notification);
    }
}
