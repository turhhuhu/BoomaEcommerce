using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Services;
using BoomaEcommerce.Services.DTO;

namespace BoomaEcommerce.Tests.CoreLib
{
    public class NotificationPublisherStub : INotificationPublisher
    {
        public Dictionary<Guid, List<NotificationDto>> _notificationsMap = new Dictionary<Guid, List<NotificationDto>>();

        public Task addNotifiedUser(Guid guid)
        {
            _notificationsMap.Add(guid, new List<NotificationDto>());
            return Task.CompletedTask;
        }

        public Task NotifyAsync(NotificationDto notification, Guid userToNotify)
        {
            _notificationsMap[userToNotify].Add(notification);
            return Task.CompletedTask;
        }

        public Task NotifyManyAsync(NotificationDto notification, IEnumerable<Guid> usersToNotify)
        {
            foreach (var user in usersToNotify)
            {
                _notificationsMap[user].Add(notification);
            }

            return Task.CompletedTask;
        }
    }
}
