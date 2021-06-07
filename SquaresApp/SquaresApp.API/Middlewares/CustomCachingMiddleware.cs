using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using SquaresApp.Common.Constants;
using SquaresApp.Common.ExtentionMethods;
using SquaresApp.Common.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SquaresApp.API.Middlewares
{

    public class CustomCachingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDistributedCache _cache;

        private const string CacheSquaresGet = "SquaresGet";
        private const string CachePointsGet = "PointsGet";
        private readonly DistributedCacheEntryOptions _distributedCacheEntryOption;
        public CustomCachingMiddleware(RequestDelegate next, IDistributedCache cache, AppSettings appSettings)
        {
            _next = next;
            _cache = cache;

            _distributedCacheEntryOption = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = appSettings.CacheConfig.AbsoluteExpirationTimeInMin > 0 ? TimeSpan.FromMinutes(appSettings.CacheConfig.AbsoluteExpirationTimeInMin) : null, SlidingExpiration = appSettings.CacheConfig.SlidingExpirationTimeInMin > 0 ? TimeSpan.FromMinutes(appSettings.CacheConfig.SlidingExpirationTimeInMin) : null };
        }

        public async Task InvokeAsync(HttpContext ctx)
        {
            var userId = ctx.User.GetUserId();

            //var controllerName = ctx.GetEndpoint()?.Metadata?.GetMetadata<ControllerActionDescriptor>()?.ControllerName; //
            var controllerName = ctx.Request.Path.ToString().Split('/', StringSplitOptions.RemoveEmptyEntries).Skip(2).FirstOrDefault()?.ToLower().Trim(); // Path e.g /api/v1/Point/... 

            var cachedValue = await GetCachedResponseAsync(ctx, userId, controllerName);

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

                await SetResponseCacheAsync(ctx, userId, controllerName);

                await responseBody.CopyToAsync(originalBodyStream);
            }
        }

        private async Task SetResponseCacheAsync(HttpContext ctx, long userId, string controllerName)
        {
            ctx.Response.Body.Seek(0, SeekOrigin.Begin);
            if (ctx.Response.StatusCode == StatusCodes.Status200OK) //only cache the successful responses.
            {

                var buffer = new byte[Convert.ToInt32(ctx.Response.Body.Length)];
                await ctx.Response.Body.ReadAsync(buffer, 0, buffer.Length);
                ctx.Response.Body.Seek(0, SeekOrigin.Begin);

                switch (controllerName)
                {
                    case "point":
                        {

                            if (ctx.Request.Method == "GET") // cache points response to prevent database roundttrip.
                            {
                                await _cache.SetAsync($"{userId}-{CachePointsGet}", buffer, _distributedCacheEntryOption);
                            }
                            else if (ctx.Request.Method == "DELETE") // invalidate/remove other point and square related cache as an existing point has been deleted.
                            {
                                await _cache.SetAsync($"{userId}-{ctx.Request.Path}", buffer, _distributedCacheEntryOption);
                                await _cache.RemoveAsync($"{userId}-{CachePointsGet}");
                                await _cache.RemoveAsync($"{userId}-{CacheSquaresGet}");
                            }
                            else if (ctx.Request.Method == "POST") // invalidate/remove other point and square related cache as one or more than one new existing points have been added.
                            {
                                await _cache.RemoveAsync($"{userId}-{CachePointsGet}");
                                await _cache.RemoveAsync($"{userId}-{CacheSquaresGet}");
                            }
                        }
                        break;
                    case "square":
                        {
                            if (ctx.Request.Method == "GET") // cache the squares identified to prevent re-identification processing as this won't change until one or more point is added or removed.
                            {
                                await _cache.SetAsync($"{userId}-{CacheSquaresGet}", buffer, _distributedCacheEntryOption);
                            }

                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private async Task<byte[]> GetCachedResponseAsync(HttpContext ctx, long userId, string controllerName)
        {
            var cachedValue = default(byte[]);
            switch (controllerName)
            {
                case "point":
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
                case "square":
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

            return cachedValue;
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
