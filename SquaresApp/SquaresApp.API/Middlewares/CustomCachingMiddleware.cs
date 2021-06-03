using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Caching.Distributed;
using SquaresApp.Common.Constants;
using SquaresApp.Common.ExtentionMethods;
using SquaresApp.Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SquaresApp.API.Middlewares
{

    public class CustomCachingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDistributedCache _cache; 

        private const string CacheSquaresGet = "SquaresGet";
        private const string CachePointsGet = "PointsGet";
        private readonly DistributedCacheEntryOptions distributedCacheEntryOption;
        public CustomCachingMiddleware(RequestDelegate next, IDistributedCache cache,AppSettings appSettings)
        {
            _next = next;
            _cache = cache; 

            distributedCacheEntryOption = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(appSettings.CacheConfig.AbsoluteExpirationTimeInMin), SlidingExpiration = TimeSpan.FromMinutes(appSettings.CacheConfig.SlidingExpirationTimeInMin) };
        }

        public async Task InvokeAsync(HttpContext ctx)
        { 
            var userId = ctx.User.GetUserId(); 
            var controllerName = ctx.GetEndpoint().Metadata.GetMetadata<ControllerActionDescriptor>().ControllerName;

            byte[] cachedValue = null;
            switch (controllerName)
            {
                case "Point":
                    { 
                        if (ctx.Request.Method == "GET")
                        {
                            cachedValue = await _cache.GetAsync($"{userId}-{CachePointsGet}");
                        }
                        else if (ctx.Request.Method == "DELETE")
                        {
                            cachedValue = await _cache.GetAsync($"{userId}-{ctx.Request.Path}");
                        }
                    }
                    break;
                case "Square":
                    {
                        if (ctx.Request.Method == "GET")
                        {
                            cachedValue = await _cache.GetAsync($"{userId}-{CacheSquaresGet}");
                        }

                    }

                    break;
                default:
                    break;
            }

            if (cachedValue is not null) //cache hit successful. So, no need to hit action method. 
            {
                ctx.Response.ContentType = ConstantValues.JSONContentType;
                await ctx.Response.Body.WriteAsync(cachedValue);
                return;
            } 

            var originalBodyStream = ctx.Response.Body;
            using var responseBody = new MemoryStream();
            {
                ctx.Response.Body = responseBody;

                await _next(ctx);


                if (ctx.Response.StatusCode == StatusCodes.Status200OK)
                {
                    ctx.Response.Body.Seek(0, SeekOrigin.Begin);
                    var buffer = new byte[Convert.ToInt32(ctx.Response.Body.Length)];
                    await ctx.Response.Body.ReadAsync(buffer, 0, buffer.Length);
                    ctx.Response.Body.Seek(0, SeekOrigin.Begin);

                    switch (controllerName)
                    {
                        case "Point":
                            {

                                if (ctx.Request.Method == "GET")
                                {
                                    await _cache.SetAsync($"{userId}-{CachePointsGet}", buffer, distributedCacheEntryOption);
                                }
                                else if (ctx.Request.Method == "DELETE")
                                {
                                    await _cache.SetAsync($"{userId}-{ctx.Request.Path}", buffer, distributedCacheEntryOption);
                                    await _cache.RemoveAsync($"{userId}-{CachePointsGet}");
                                    await _cache.RemoveAsync($"{userId}-{CacheSquaresGet}");
                                }
                                else if (ctx.Request.Method == "POST")
                                {
                                    await _cache.RemoveAsync($"{userId}-{CachePointsGet}");
                                    await _cache.RemoveAsync($"{userId}-{CacheSquaresGet}");
                                }
                            }
                            break;
                        case "Square":
                            {
                                if (ctx.Request.Method == "GET")
                                {
                                    if (ctx.Response.StatusCode == StatusCodes.Status200OK)
                                    {
                                        await _cache.SetAsync($"{userId}-{CacheSquaresGet}", buffer, distributedCacheEntryOption);
                                    }
                                }

                            }
                            break;
                        default:
                            break;
                    }
                }

                await responseBody.CopyToAsync(originalBodyStream);
            }
        }

    }





    public static class CustomCachingMiddlewareExt
    {
        /// <summary>
        /// middleware responsible for caching responses
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseCustomCaching(this IApplicationBuilder app)
        {
            return app.UseMiddleware<CustomCachingMiddleware>();

        }
    }
}
