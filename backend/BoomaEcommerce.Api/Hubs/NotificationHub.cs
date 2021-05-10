#nullable enable
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
    public class NotificationHub : Hub<INotificationClient>
    {
        private readonly ILogger<NotificationHub> _logger;
        private readonly IConnectionContainer _connectionContainer;

        public NotificationHub(ILogger<NotificationHub> logger, IConnectionContainer connectionContainer)
        {
            _logger = logger;
            _connectionContainer = connectionContainer;
        }

        public override Task OnConnectedAsync()
        {
            var userGuid = Context.User.GetUserGuid();
            _logger.LogInformation($"User with Guid {userGuid} connected to notifications hub.");
            _connectionContainer.SaveConnection(userGuid, Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var userGuid = Context.User.GetUserGuid();
            _logger.LogInformation($"User with Guid {userGuid} disconnected from notifications hub.");
            _connectionContainer.RemoveConnection(userGuid);
            return base.OnDisconnectedAsync(exception);
        }

    }
}
