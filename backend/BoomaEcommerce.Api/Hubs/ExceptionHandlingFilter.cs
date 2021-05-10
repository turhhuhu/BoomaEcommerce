using BoomaEcommerce.Core;
using BoomaEcommerce.Core.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoomaEcommerce.Api.Hubs
{
    public class ExceptionHandlingFilter : IHubFilter
    {
        private readonly ILogger<ExceptionHandlingFilter> _logger;

        public ExceptionHandlingFilter(ILogger<ExceptionHandlingFilter> logger) 
        {
            _logger = logger;
        }
        public ValueTask<object> InvokeMethod(HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object>> next)
        {
            _logger.LogInformation($"Calling hub method '{invocationContext.HubMethodName}'");
            try
            {
                return next(invocationContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to call hub method '{invocationContext.HubMethodName}'");
                throw;
            }
        }

        public Task OnConnectedAsync(HubLifetimeContext context, Func<HubLifetimeContext, Task> next)
        {
            try
            {
                context.Context.User.GetUserGuid();
                return next(context);
            }
            catch (UnAuthenticatedException)
            {
                _logger.LogInformation("OOpsiess");
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Failed to connect {context.Context.User.GetUserGuid()} to hub.");
                throw;
            }
        }
    }
}
