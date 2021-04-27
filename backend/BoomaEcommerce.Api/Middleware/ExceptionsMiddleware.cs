using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Core.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BoomaEcommerce.Api.Middleware
{
    public class ExceptionsMiddleware
    {
        private readonly ILogger<ExceptionsMiddleware> _logger;
        private readonly RequestDelegate _next;

        public ExceptionsMiddleware(ILogger<ExceptionsMiddleware> logger, RequestDelegate next)
        {
            this._logger = logger;
            this._next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (UnAuthorizedException)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("User unauthorized for resource.");
            }
            catch (UnAuthenticatedException)
            {
                await context.ForbidAsync();
            }
            catch (ValidationException validationException)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(validationException.Errors.Select(x => x.ErrorMessage));
            }
            catch (Exception exception)
            {
                _logger.LogError(exception,
                    $"Request {context.Request?.Method}: {context.Request?.Path.Value} failed");
            }
        }
    }
}
