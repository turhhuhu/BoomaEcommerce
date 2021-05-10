using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Api.Hubs;
using BoomaEcommerce.Services;
using BoomaEcommerce.Services.DTO;
using Microsoft.AspNetCore.SignalR;

namespace BoomaEcommerce.Api
{
    public class NotificationPublisher : INotificationPublisher
    {
        private readonly IConnectionContainer _connectionContainer;
        private readonly IHubContext<NotificationHub, INotificationClient> _notificationHub;
        public NotificationPublisher(IConnectionContainer connectionContainer,
            IHubContext<NotificationHub, INotificationClient> notificationHub)
        {
            _connectionContainer = connectionContainer;
            _notificationHub = notificationHub;
        }

        public Task NotifyAsync(NotificationDto notification, Guid userToNotify)
        {
            var connectionId = _connectionContainer.GetConnectionId(userToNotify);
            return connectionId != null
                ? _notificationHub.Clients.Client(connectionId).ReceiveNotification(notification)
                : Task.CompletedTask;
        }

        public Task NotifyManyAsync(NotificationDto notification, IEnumerable<Guid> usersToNotify)
        {
            return Task.WhenAll(usersToNotify.Select(userGuid => NotifyAsync(notification, userGuid)));
        }

    }
}
