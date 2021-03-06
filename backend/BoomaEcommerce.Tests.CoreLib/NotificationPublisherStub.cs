using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services;
using BoomaEcommerce.Services.DTO;

namespace BoomaEcommerce.Tests.CoreLib
{
    public class NotificationPublisherStub : INotificationPublisher
    {
        public Dictionary<Guid, List<NotificationDto>> _notificationsMap = new Dictionary<Guid, List<NotificationDto>>();

        public Task addNotifiedUser(Guid guid, List<NotificationDto> notifications)
        {
            _notificationsMap.Add(guid, notifications);
            return Task.CompletedTask;
        }

        public Task NotifyAsync(NotificationDto notification, Guid userToNotify)
        {
            if (_notificationsMap.ContainsKey(userToNotify))
            {
                _notificationsMap[userToNotify].Add(notification);
            }
            return Task.CompletedTask;
        }

        public Task NotifyManyAsync(IEnumerable<(Guid userGuid, NotificationDto notification)> userNotificationPairs)
        {
            foreach (var (userGuid, notification) in userNotificationPairs)
            {
                if (_notificationsMap.ContainsKey(userGuid))
                {
                    _notificationsMap[userGuid].Add(notification);
                }
            }

            return Task.CompletedTask;
        }
    }
}
