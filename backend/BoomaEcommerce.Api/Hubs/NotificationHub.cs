using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Services;
using BoomaEcommerce.Services.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace BoomaEcommerce.Api.Hubs
{
    [Authorize(Policy = "TokenHasUserGuidPolicy")]
    public class NotificationHub : Hub<INotificationClient>, INotificationHub
    {
        private readonly ConcurrentDictionary<Guid, string> _userGuidToConnectionIdMapping;
        private readonly ILogger<NotificationHub> _logger;

        public NotificationHub(ILogger<NotificationHub> logger)
        {
            _userGuidToConnectionIdMapping = new ConcurrentDictionary<Guid, string>();
            _logger = logger;
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

        public Task NotifyAsync(NotificationDto notification, Guid userToNotify)
        {
            return _userGuidToConnectionIdMapping.TryGetValue(userToNotify, out var connectionId) 
                ? Clients.Client(connectionId).ReceiveNotification(notification) 
                : Task.CompletedTask;
        }

        public Task NotifyManyAsync(NotificationDto notification, IEnumerable<Guid> usersToNotify)
        {
            return Task.WhenAll(usersToNotify.Select(userGuid => NotifyAsync(notification, userGuid)));
        }

    }
}
