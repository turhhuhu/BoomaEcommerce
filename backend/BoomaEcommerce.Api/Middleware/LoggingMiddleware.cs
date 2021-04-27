using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
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
            context.Request.EnableBuffering();

            if (!context.Request.Method.Equals("GET"))
            {
                var reader = new StreamReader(context.Request.Body);

                var requestBody = await reader.ReadToEndAsync();
                var indentedRequestBody = JToken.Parse(requestBody).ToString(Formatting.Indented);

                _logger.LogInformation(
                    $"Received request number: {_requestCounter}\nRequest {context.Request?.Method}: {context.Request?.Path.Value}\n{indentedRequestBody}");
                context.Request.Body.Position = 0L;
            }

            var originalResponseStream = context.Response.Body;
            var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;

            await _next(context);

            context.Response.Body.Position = 0L;
            var responseReader = new StreamReader(context.Response.Body);
            var responseBody = await responseReader.ReadToEndAsync();

            var indentedResponseBody = JToken.Parse(responseBody).ToString(Formatting.Indented);
            context.Response.Body.Position = 0L;

            await responseBodyStream.CopyToAsync(originalResponseStream);
            _logger.LogInformation(
                $"Responding to request number: {_requestCounter}\nResponse status code:{context.Response.StatusCode}\nResponse:\n{indentedResponseBody}");
            _requestCounter++;
        }
    }
}
