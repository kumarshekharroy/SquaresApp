using Microsoft.AspNetCore.Http;
using SquaresApp.Common.Constants;
using System;

namespace SquaresApp.Common.ExtentionMethods
{
    public static class HttpContextExtentions
    {

        /// <summary>
        /// extention over httpcontext. check for existance of correlationIDHeader and if not found then it creates one.
        /// </summary>
        /// <param name="httpContext"></param> 
        /// <returns></returns>
        public static string SetCorrelationIdHeader(this HttpContext httpContext)
        {
            string correlationID;

            if (!httpContext.Request.Headers.TryGetValue(ConstantValues.CorrelationIdHeader, out var _correlationID) || string.IsNullOrWhiteSpace(_correlationID))
            {
                correlationID = Guid.NewGuid().ToString();
                httpContext.Request.Headers[ConstantValues.CorrelationIdHeader] = correlationID;
            }
            else
            {
                correlationID = _correlationID.ToString();
            }

            httpContext.Response.Headers[ConstantValues.CorrelationIdHeader] = correlationID;
            httpContext.Items[ConstantValues.CorrelationIdHeader] = correlationID;
            return correlationID;
        }
    }
}
