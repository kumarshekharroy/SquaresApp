using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using SquaresApp.Common.Constants;
using SquaresApp.Common.Models;
using System.Text.Json;
using System.Threading.Tasks;

namespace SquaresApp.API.Middlewares
{
    public class CustomUnauthorizedResponseMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomUnauthorizedResponseMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext ctx)
        {
            await _next(ctx);

            if (ctx.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                ctx.Response.ContentType = ConstantValues.JSONContentType;
                var response = new Response<string> { Message = ConstantValues.UnauthorizedRequestMessage };
                await ctx.Response.Body.WriteAsync(JsonSerializer.SerializeToUtf8Bytes(response));
            }

        }

    }



    public static class CustomUnauthorizedResponseMiddlewareExtention
    {
        /// <summary>
        /// middleware to set content-location header on desired request
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseCustomUnauthorizedResponse(this IApplicationBuilder app)
        {
            return app.UseMiddleware<CustomUnauthorizedResponseMiddleware>();
        }
    }
}
