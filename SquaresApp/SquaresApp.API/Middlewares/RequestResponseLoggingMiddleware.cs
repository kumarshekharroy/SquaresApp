using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Context;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

namespace SquaresApp.API.Middlewares
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {

            var stopWatch = Stopwatch.StartNew();

            await LogRequestInformationsAsync(context.Request);

            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            {
                context.Response.Body = responseBody;
                await _next(context);
                stopWatch.Stop();
                await LogResponseInformationsAsync(context.Response, stopWatch.ElapsedMilliseconds);

                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
            }

        }
        private async Task LogRequestInformationsAsync(HttpRequest request)
        {
            request.EnableBuffering();
              
            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer);
            var bodyAsText = Encoding.UTF8.GetString(buffer);

            request.Body.Seek(0, SeekOrigin.Begin); 

            var requestInfo = new  { Scheme = request.Scheme, Method = request.Method, Host = request.Host.Value, Path = request.Path, QueryString = request.QueryString.Value, Body = bodyAsText, Headers = request.Headers.ToDictionary(a => a.Key, a => string.Join(";", a.Value.ToArray())) };

            _logger.LogDebug("Request Info {@RequestInfo}", requestInfo);

        }

        private async Task LogResponseInformationsAsync(HttpResponse response, long execTime = -1)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var bodyAsText = await new StreamReader(response.Body).ReadToEndAsync();

            var responseInfo = new  { StatusCode = response.StatusCode, Body = bodyAsText, ExecutionTimeInMilliSec = execTime, Headers = response.Headers.ToDictionary(a => a.Key, a => string.Join(";", a.Value.ToArray())) };

            _logger.LogDebug("Response Info {@ResponseInfo}", responseInfo);

        }
    }
    public static class RequestResponseLoggingMiddlewareExtention
    {
        /// <summary>
        /// middleware to log request and response.
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestResponseLoggingMiddleware>();
        }
         
    }
}
