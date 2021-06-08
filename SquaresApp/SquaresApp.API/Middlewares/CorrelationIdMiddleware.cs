using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Serilog.Context;
using SquaresApp.Common.Constants;
using SquaresApp.Common.ExtentionMethods;
using System.Threading.Tasks; 

namespace SquaresApp.API.Middlewares
{

    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext ctx)
        {

            var correlationID = ctx.SetCorrelationIdHeader();

            using (LogContext.PushProperty(ConstantValues.CorrelationIdHeader, correlationID))
            {
                await _next(ctx);
            }

        }
    }

    public static class CorrelationIdMiddlewareExt
    {
        /// <summary>
        /// middleware to check and set correlation id for every request
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
        {
            return app.UseMiddleware<CorrelationIdMiddleware>();
        }
    }
}
