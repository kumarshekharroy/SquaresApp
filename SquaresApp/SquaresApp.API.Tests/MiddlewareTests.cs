using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SquaresApp.API.Middlewares;
using SquaresApp.Common.Constants;
using SquaresApp.Common.Models;
using System;
using System.IO;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace SquaresApp.API.Tests
{
    public class MiddlewareTests
    {

        private const string UserId = "1";
        private readonly DefaultHttpContext _defaultHttpContext;
        private readonly AppSettings _appSettings;
        private readonly IDistributedCache _distributedCache;

        public MiddlewareTests()
        {

            var fakeIdentity = new GenericIdentity("User");
            fakeIdentity.AddClaim(new Claim(ConstantValues.UserId, UserId));
            var principal = new GenericPrincipal(fakeIdentity, null);

            _defaultHttpContext = new DefaultHttpContext() { User=principal}; 
            _defaultHttpContext.Response.Body = new MemoryStream();

            var cacheConfig = new CacheConfig { AbsoluteExpirationTimeInMin = 2, SlidingExpirationTimeInMin = 1 };
            _appSettings = new AppSettings { CacheConfig = cacheConfig };

            var opts = Options.Create<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions());
            _distributedCache = new MemoryDistributedCache(opts);
        }


        /// <summary>
        /// Test ensures that the successful get points response has been cached.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task CustomCacheMiddleware_GetPointStatus200()
        {
            // Arrange 

            const string CacheSuffix = "PointsGet";
            var cacheKey = $"{UserId}-{CacheSuffix}";
            var expectedResponseString = "Some response.";

            _defaultHttpContext.Request.Path = "/api/v1/Points";
            _defaultHttpContext.Request.Method = "GET";


            // Act
            var middlewareInstance = new CustomCachingMiddleware(next: (innerHttpContext) =>
            {
                _defaultHttpContext.Response.StatusCode = StatusCodes.Status200OK;
                _defaultHttpContext.Response.Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(expectedResponseString));
                return Task.CompletedTask;
            }, cache: _distributedCache, appSettings: _appSettings);

            await middlewareInstance.InvokeAsync(_defaultHttpContext);

            var cachedData = await _distributedCache.GetStringAsync(cacheKey);

            //Assert    
            Assert.NotNull(cachedData);
            Assert.Equal(cachedData, expectedResponseString);

        }


        /// <summary>
        /// Test ensures that the unsuccessful get points response has not been cached.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task CustomCacheMiddleware_GetPointStatusNon200()
        {
            // Arrange 

            const string CacheSuffix = "PointsGet";
            var cacheKey = $"{UserId}-{CacheSuffix}";
            var expectedResponseString = "Some response.";

            _defaultHttpContext.Request.Path = "/api/v1/Points";
            _defaultHttpContext.Request.Method = "GET";

            var opts = Options.Create<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions());
            var distributedCache = new MemoryDistributedCache(opts);

            // Act
            var middlewareInstance = new CustomCachingMiddleware(next: (innerHttpContext) =>
            {
                _defaultHttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                _defaultHttpContext.Response.Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(expectedResponseString));
                return Task.CompletedTask;
            }, cache: distributedCache, appSettings: _appSettings);

            await middlewareInstance.InvokeAsync(_defaultHttpContext);

            var cachedData = await distributedCache.GetStringAsync(cacheKey);

            //Assert    
            Assert.Null(cachedData);

        }



        /// <summary>
        /// Test ensures that the successful get squares response has been cached.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task CustomCacheMiddleware_GetSquaresStatus200()
        {
            // Arrange 

            const string CacheSuffix = "SquaresGet";
            var cacheKey = $"{UserId}-{CacheSuffix}";
            var expectedResponseString = "Some response.";

            _defaultHttpContext.Request.Path = "/api/v1/Squares";
            _defaultHttpContext.Request.Method = "GET";


            // Act
            var middlewareInstance = new CustomCachingMiddleware(next: (innerHttpContext) =>
            {
                _defaultHttpContext.Response.StatusCode = StatusCodes.Status200OK;
                _defaultHttpContext.Response.Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(expectedResponseString));
                return Task.CompletedTask;
            }, cache: _distributedCache, appSettings: _appSettings);

            await middlewareInstance.InvokeAsync(_defaultHttpContext);

            var cachedData = await _distributedCache.GetStringAsync(cacheKey);

            //Assert    
            Assert.NotNull(cachedData);
            Assert.Equal(cachedData, expectedResponseString);

        }


        /// <summary>
        /// Test ensures that the unsuccessful get squares response has not been cached.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task CustomCacheMiddleware_GetSquaresStatusNon200()
        {
            // Arrange 

            const string CacheSuffix = "SquaresGet";
            var cacheKey = $"{UserId}-{CacheSuffix}";
            var expectedResponseString = "Some response.";

            _defaultHttpContext.Request.Path = "/api/v1/Squares";
            _defaultHttpContext.Request.Method = "GET";


            // Act
            var middlewareInstance = new CustomCachingMiddleware(next: (innerHttpContext) =>
            {
                _defaultHttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                _defaultHttpContext.Response.Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(expectedResponseString));
                return Task.CompletedTask;
            }, cache: _distributedCache, appSettings: _appSettings);

            await middlewareInstance.InvokeAsync(_defaultHttpContext);

            var cachedData = await _distributedCache.GetStringAsync(cacheKey);

            //Assert    
            Assert.Null(cachedData);
        }

        /// <summary>
        /// Test ensures that the successful delete point response has been cached and invalidated/removed existing getpoints and getsquares cache.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task CustomCacheMiddleware_DeletePointStatus200()
        {
            // Arrange 

            const string getPointCacheSuffix = "PointsGet";
            var getPointsCacheKey = $"{UserId}-{getPointCacheSuffix}";
            var existingGetPointsCacheData = "Some points response.";

            const string getSquaresCacheSuffix = "SquaresGet";
            var getSquaresCacheKey = $"{UserId}-{getSquaresCacheSuffix}";
            var existingGetSquaresCacheData = "Some squares response.";

            var expectedDeletedPointCacheData = "Successfully deleted response.";

            _distributedCache.SetString(getPointsCacheKey, existingGetPointsCacheData);
            _distributedCache.SetString(getSquaresCacheKey, existingGetSquaresCacheData);

            _defaultHttpContext.Request.Path = "/api/v1/Points/1";
            _defaultHttpContext.Request.Method = "DELETE";


            // Act
            var middlewareInstance = new CustomCachingMiddleware(next: (innerHttpContext) =>
            {
                _defaultHttpContext.Response.StatusCode = StatusCodes.Status200OK;
                _defaultHttpContext.Response.Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(expectedDeletedPointCacheData));
                return Task.CompletedTask;
            }, cache: _distributedCache, appSettings: _appSettings);

            await middlewareInstance.InvokeAsync(_defaultHttpContext);

            var getPointsCacheData = await _distributedCache.GetStringAsync(getPointsCacheKey);
            var getSquaresCacheData = await _distributedCache.GetStringAsync(getSquaresCacheKey);
            var deletedPointCacheData = await _distributedCache.GetStringAsync($"{UserId}-{_defaultHttpContext.Request.Path}");

            //Assert    
            Assert.Null(getPointsCacheData);
            Assert.Null(getSquaresCacheData);
            Assert.NotNull(deletedPointCacheData);
            Assert.Equal(deletedPointCacheData, expectedDeletedPointCacheData);

        }

        /// <summary>
        /// Test ensures that the unsuccessful delete point response has not been cached and existing getpoints and getsquares cache has not been invalidated/removed.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task CustomCacheMiddleware_DeletePointStatusNon200()
        {
            // Arrange 

            const string getPointCacheSuffix = "PointsGet";
            var getPointsCacheKey = $"{UserId}-{getPointCacheSuffix}";
            var existingGetPointsCacheData = "Some points response.";

            const string getSquaresCacheSuffix = "SquaresGet";
            var getSquaresCacheKey = $"{UserId}-{getSquaresCacheSuffix}";
            var existingGetSquaresCacheData = "Some squares response.";

            var expectedDeletedPointCacheData = "Successfully deleted response.";

            _distributedCache.SetString(getPointsCacheKey, existingGetPointsCacheData);
            _distributedCache.SetString(getSquaresCacheKey, existingGetSquaresCacheData);

            _defaultHttpContext.Request.Path = "/api/v1/Points/1";
            _defaultHttpContext.Request.Method = "DELETE";


            // Act
            var middlewareInstance = new CustomCachingMiddleware(next: (innerHttpContext) =>
            {
                _defaultHttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                _defaultHttpContext.Response.Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(expectedDeletedPointCacheData));
                return Task.CompletedTask;
            }, cache: _distributedCache, appSettings: _appSettings);

            await middlewareInstance.InvokeAsync(_defaultHttpContext);

            var getPointsCacheData = await _distributedCache.GetStringAsync(getPointsCacheKey);
            var getSquaresCacheData = await _distributedCache.GetStringAsync(getSquaresCacheKey);
            var deletedPointCacheData = await _distributedCache.GetStringAsync($"{UserId}-{_defaultHttpContext.Request.Path}");

            //Assert    
            Assert.NotNull(getPointsCacheData);
            Assert.Equal(getPointsCacheData, existingGetPointsCacheData);
            Assert.NotNull(getSquaresCacheData);
            Assert.Equal(getSquaresCacheData, existingGetSquaresCacheData);
            Assert.Null(deletedPointCacheData);

        }

        /// <summary>
        /// Test ensures that response contains the same correlation header which was supplied by client in request header
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task CorrelationIdMiddlewareTest_HeaderSuppliedByClient()
        {
            // Arrange
            var CorrelationIDHeader = Guid.NewGuid().ToString();
            _defaultHttpContext.Request.Headers[ConstantValues.CorrelationIdHeader] = CorrelationIDHeader;

            // Act
            var middlewareInstance = new CorrelationIdMiddleware(next: (innerHttpContext) =>
            {
                return Task.CompletedTask;
            });

            await middlewareInstance.InvokeAsync(_defaultHttpContext);


            //Assert
            Assert.True(_defaultHttpContext.Response.Headers.ContainsKey(ConstantValues.CorrelationIdHeader));
            Assert.True(_defaultHttpContext.Response.Headers[ConstantValues.CorrelationIdHeader].ToString() == CorrelationIDHeader);
        }

        /// <summary>
        /// Test ensures that response contains the a correlation header even when no correlation header was supplied by client in request
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task CorrelationIdMiddlewareTest_HeaderNotSuppliedByClient()
        {
            // Arrange   

            // Act
            var middlewareInstance = new CorrelationIdMiddleware(next: (innerHttpContext) =>
            {
                return Task.CompletedTask;
            });

            await middlewareInstance.InvokeAsync(_defaultHttpContext);


            //Assert
            Assert.True(_defaultHttpContext.Response.Headers.ContainsKey(ConstantValues.CorrelationIdHeader));
            Assert.True(Guid.TryParse(_defaultHttpContext.Response.Headers[ConstantValues.CorrelationIdHeader].ToString(), out var _));
        }

        /// <summary>
        /// Test ensures that the custom exception handler middleware is handling and logging exceptions
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ExceptionHandlerMiddlewareTest()
        {
            // Arrange   
            Mock<ILogger<ExceptionHandlerMiddleware>> mockLogger = new Mock<ILogger<ExceptionHandlerMiddleware>>(); 

            // Act
            var middlewareInstance = new ExceptionHandlerMiddleware(next: (innerHttpContext) =>
            {
                throw new Exception("Timeout occured");
            }, logger: mockLogger.Object);

            await middlewareInstance.InvokeAsync(_defaultHttpContext);


            //Assert
            Assert.True(_defaultHttpContext.Response.ContentType == ConstantValues.JSONContentType);
            mockLogger.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
        }

        /// <summary>
        /// Test ensures that the request/response logging middleware is working as intended
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task RequestResponseLoggingMiddlewareTest()
        {
            // Arrange  
            Mock<ILogger<RequestResponseLoggingMiddleware>> mockLogger = new Mock<ILogger<RequestResponseLoggingMiddleware>>();

            _defaultHttpContext.Request.Path = "/Points";

            // Act
            var middlewareInstance = new RequestResponseLoggingMiddleware(next: (innerHttpContext) =>
            {
                return Task.CompletedTask;
            }, logger: mockLogger.Object);

            await middlewareInstance.InvokeAsync(_defaultHttpContext);

            //Assert  
            mockLogger.Verify(x => x.Log(LogLevel.Debug, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Exactly(2));
        }

        /// <summary>
        /// Test ensures that the custom response is being sent for unauthorized request.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task CustomUnauthorizedResponsePayloadMiddlewareTest()
        {
            // Arrange 
            DefaultHttpContext defaultContext = new DefaultHttpContext();
            defaultContext.Response.Body = new MemoryStream();
            defaultContext.Response.StatusCode = StatusCodes.Status401Unauthorized;

            // Act
            var middlewareInstance = new CustomUnauthorizedResponseMiddleware(next: (innerHttpContext) =>
            {
                return Task.CompletedTask;
            });

            await middlewareInstance.InvokeAsync(defaultContext);

            defaultContext.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBodyAsText = await new StreamReader(defaultContext.Response.Body).ReadToEndAsync();

            var deserializedResponse =JsonSerializer.Deserialize<Response<string>>(responseBodyAsText);

            //Assert
            Assert.True(defaultContext.Response.ContentType == ConstantValues.JSONContentType);
            Assert.NotNull(deserializedResponse);
            Assert.Equal(deserializedResponse.Message, ConstantValues.UnauthorizedRequestMessage);
        }
    }
}
