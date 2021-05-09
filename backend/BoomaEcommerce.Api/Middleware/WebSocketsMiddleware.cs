using BoomaEcommerce.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoomaEcommerce.Api.Middleware
{
    public class WebSocketsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<WebSocketsMiddleware> _logger;

        public WebSocketsMiddleware(RequestDelegate next, ILogger<WebSocketsMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var request = httpContext.Request;
            // web sockets cannot pass headers so we must take the access token from query param and
            // add it to the header before authentication middleware runs
            if (request.Path.StartsWithSegments("/hub", StringComparison.OrdinalIgnoreCase))
            {
                //if (!httpContext.User.TryGetUserGuid(out _))
                //{
                //    await httpContext.ForbidAsync();
                //    return;
                //}
                if(request.Query.TryGetValue("access_token", out var accessToken))
                {
                    request.Headers.Add("Authorization", $"Bearer {accessToken}");
                }
            }
            await _next(httpContext);
        }
    }
}
