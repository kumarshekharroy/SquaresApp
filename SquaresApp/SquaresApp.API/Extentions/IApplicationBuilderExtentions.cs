using Microsoft.AspNetCore.Builder;
using SquaresApp.Common.Constants;

namespace SquaresApp.API.Extentions
{
    public static class IApplicationBuilderExtentions
    {
        /// <summary>
        /// use Wildcard cors policy
        /// </summary>
        /// <param name="app"></param> 
        /// <returns></returns>
        public static IApplicationBuilder UseWildcardCors(this IApplicationBuilder app)
        {
            app.UseCors(ConstantValues.AllowAllOriginsCorsPolicy);
            return app;
        }

        /// <summary>
        /// configure route for Swagger docs
        /// </summary>
        /// <param name="app"></param> 
        /// <returns></returns>
        public static IApplicationBuilder UseSwaggerDoc(this IApplicationBuilder app)
        {
            app.UseSwagger().UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SquaresApp.API v1");
                c.RoutePrefix = string.Empty;
            });

            return app;
        }

    }
}
