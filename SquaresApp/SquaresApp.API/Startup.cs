using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SquaresApp.API.Extentions;
using SquaresApp.API.Middlewares;
using SquaresApp.Application.Profiles;
using SquaresApp.Common.Constants;
using SquaresApp.Data.Context;

namespace SquaresApp.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddServicesAndRepositories();

            services.AddDbContext<SquaresAppDBContext>(options => options.UseSqlServer(Configuration.GetConnectionString(ConstantValues.DbConnString)));

            services.AddAutoMapper(config => config.AddProfile<AutoMapperProfiles>());

            services.AddAppSettings(Configuration);

            services.AddJWTAuthentication(Configuration);

            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                options.JsonSerializerOptions.IgnoreNullValues = true;
            });

            services.AddSwagger();

            services.AddDistributedCaching(Configuration);

            services.AddWildcardCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseCorrelationId();

            app.UseRequestResponseLogging();

            app.UseCustomExceptionHandler();

            app.UseHttpsRedirection();

            app.UseWildcardCors();

            app.UseRouting();

            app.UseSwaggerDoc();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseCustomCaching();
             
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
