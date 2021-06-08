using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SquaresApp.Common.Constants;
using SquaresApp.Common.Models;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace SquaresApp.API.Middlewares
{


    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext ctx)
        {

            try
            {
                await _next(ctx);
            }
            catch (Exception ex)
            {
                await HandleException(ctx, ex);
            }

        }
        private async Task HandleException(HttpContext ctx, Exception ex)
        {
            ctx.Response.ContentType = ConstantValues.JSONContentType;
            ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var response = new Response<object> { Message = ConstantValues.UnexpectedErrorMessage, Data = new { Error = new { Message = ex.InnerException?.Message ?? ex.Message, Type = ex.GetType().ToString() } } };

            await ctx.Response.Body.WriteAsync(JsonSerializer.SerializeToUtf8Bytes(response));

            _logger.LogError(ex, "Exception Info ");

        }
    }


    public static class ExceptionHandlerMiddlewareExtention
    {

        /// <summary>
        /// middleware to handle and log exception globally
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static void UseCustomExceptionHandler(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionHandlerMiddleware>();
        }



    }
}
