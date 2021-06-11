#nullable enable
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Services;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;

namespace BoomaEcommerce.Api.Hubs
{
    [Authorize(Policy = "TokenHasUserGuidPolicy")]
    public class NotificationHub : Hub<INotificationClient>
    {
        private readonly ILogger<NotificationHub> _logger;
        private readonly IConnectionContainer _connectionContainer;
        private readonly JwtSettings _jwtSettings;

        public NotificationHub(ILogger<NotificationHub> logger, IConnectionContainer connectionContainer, IOptions<JwtSettings> jwtSettings)
        {
            _logger = logger;
            _connectionContainer = connectionContainer;
            _jwtSettings = jwtSettings.Value;
        }

        public override Task OnConnectedAsync()
        {
            var userGuid = Context.User.GetUserGuid();
            _logger.LogInformation($"User with Guid {userGuid} connected to notifications hub.");
            _connectionContainer.SaveConnection(userGuid, Context.ConnectionId);

            TerminateConnectionOnTokenExpiry();

            return base.OnConnectedAsync();
        }

        private void TerminateConnectionOnTokenExpiry()
        {
            var context = Context.GetHttpContext();
            var feature = Context.GetHttpContext()?.Features.Get<IConnectionHeartbeatFeature>();
            if (feature == null)
            {
                return;
            }

            if (context == null)
            {
                throw new InvalidOperationException("The HTTP context cannot be resolved.");
            }

            // Extract the authentication ticket from the access token.
            // Note: this operation should be cheap as the authentication result
            // was already computed when SignalR invoked the authentication handler
            // and automatically cached by AuthenticationHandler.AuthenticateAsync().
            var connectionId = Context.ConnectionId;
            var expirationClaim = Context.User?.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp)?.Value;
            if (expirationClaim == null)
            {
                _logger.LogWarning($"Token for connection id {connectionId} is missing expiry time, aborting.");
                Context.Abort();
                return;
            }
            var expirationDateUnix = long.Parse(expirationClaim);
            var expirationDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expirationDateUnix);

            feature.OnHeartbeat(state =>
            {
                var (exp, httpContext) = ((DateTime, HttpContext))state;

                if (exp < DateTime.UtcNow)
                {
                    _logger.LogInformation($"Token for connection id {connectionId} has expired, aborting.");
                    httpContext.Abort();
                }
                // Ensure the access token token is still valid.
                // If it's not, abort the connection immediately.
            }, (expirationDateTimeUtc, context));
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
