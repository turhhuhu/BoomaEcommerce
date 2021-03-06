using System;
using System.Collections.Generic;
using System.IO;
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
            catch (PolicyValidationException policyException)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(policyException.PolicyErrors);
            }
            catch (PaymentFailureException paymentFailureException)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(paymentFailureException.Message);
            }
            catch (SupplyFailureException supplyFailureException )
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(supplyFailureException.Message);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception,
                    $"Request {context.Request?.Method}: {context.Request?.Path.Value} failed");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync("Unexpected error has occured");
            }
        }
    }
}
