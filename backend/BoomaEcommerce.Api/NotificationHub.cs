using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Services;
using BoomaEcommerce.Services.DTO;
using Microsoft.AspNetCore.SignalR;

namespace BoomaEcommerce.Api
{
    public class NotificationHub : Hub<INotificationClient>, INotificationHub
    {
        private readonly ConcurrentDictionary<Guid, string> _userGuidToConnectionIdMapping;

        public NotificationHub()
        {
            _userGuidToConnectionIdMapping = new ConcurrentDictionary<Guid, string>();
        }

        public override Task OnConnectedAsync()
        {
            var useGuid = Context.User.GetUserGuid();
            _userGuidToConnectionIdMapping[useGuid] = Context.ConnectionId;
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var useGuid = Context.User.GetUserGuid();
            _userGuidToConnectionIdMapping.Remove(useGuid, out _);
            return base.OnDisconnectedAsync(exception);
        }

        public Task NotifyAsync(NotificationDto notification)
        {
            return _userGuidToConnectionIdMapping.TryGetValue(notification.NotifiedUserGuid, out var connectionId) 
                ? Clients.Client(connectionId).ReceiveNotification(notification) 
                : Task.CompletedTask;
        }

        public Task NotifyAsync(IEnumerable<NotificationDto> notifications)
        {
            return Task.WhenAll(notifications.Select(NotifyAsync));
        }
    }
}
