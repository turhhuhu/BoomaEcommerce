using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonConverter = System.Text.Json.Serialization.JsonConverter;

namespace BoomaEcommerce.Api.Middleware
{
    public class LoggingMiddleware
    {
        private readonly ILogger<LoggingMiddleware> _logger;
        private readonly RequestDelegate _next;
        private static int _requestCounter = 0;
        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var path = context.Request?.Path.Value;
            if (path?.EndsWith(ApiRoutes.Auth.Login) == true || path?.EndsWith(ApiRoutes.Auth.Register) == true)
            {
                await _next(context);
                return;
            }

            try
            {
                await LogRequest(context.Request);

                var originalResponseStream = context.Response.Body;

                await using var responseBodyStream = new MemoryStream();

                context.Response.Body = responseBodyStream;
                await responseBodyStream.CopyToAsync(originalResponseStream);

                await _next(context);

                await LogResponse(context.Response);

                await responseBodyStream.CopyToAsync(originalResponseStream);
            }
            catch (Exception)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            _requestCounter++;
        }

        public async Task LogRequest(HttpRequest request)
        {
            request.EnableBuffering();

            if (!request.Method.Equals("GET"))
            {
                var reader = new StreamReader(request.Body);

                var requestBody = await reader.ReadToEndAsync();
                var indentedRequestBody = JToken.Parse(requestBody).ToString(Formatting.Indented);

                _logger.LogInformation(
                    $"Received request number: {_requestCounter}\nRequest {request.Method}: {request.Path.Value}\n{indentedRequestBody}");
                request.Body.Position = 0L;
                return;
            }
            _logger.LogInformation(
                $"Received request number: {_requestCounter}\nRequest {request.Method}: {request.Path.Value}\n");
        }

        public async Task LogResponse(HttpResponse response)
        {
            response.Body.Position = 0L;
            var responseReader = new StreamReader(response.Body);
            var responseBody = await responseReader.ReadToEndAsync();

            if (responseBody.IsNullOrEmpty())
            {
                _logger.LogInformation(
                    $"Responding to request number: {_requestCounter}\nResponse status code:{response.StatusCode}");
                return;
            }

            var indentedResponseBody = JToken.Parse(responseBody).ToString(Formatting.Indented);
            response.Body.Position = 0L;

            _logger.LogInformation(
                $"Responding to request number: {_requestCounter}\nResponse status code:{response.StatusCode}\nResponse:\n{indentedResponseBody}");
        }
    }
}
