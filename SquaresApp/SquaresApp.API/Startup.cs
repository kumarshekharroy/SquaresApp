using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using SquaresApp.Common.Constants;
using SquaresApp.Data.Context;
using SquaresApp.Data.Repositories;
using SquaresApp.Domain.IRepositories;
using SquaresApp.Infra.IServices;
using SquaresApp.Infra.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SquaresApp.Infra.Profiles;
using Microsoft.Extensions.Options;
using SquaresApp.Common.Models;

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
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddDbContext<SquaresAppDBContext>(options => options.UseSqlServer(Configuration.GetConnectionString(ConstantValues.DbConnString)));

            services.AddAutoMapper(config => config.AddProfile<AutoMapperProfiles>());


            // Bind the configuration using IOptions
            services.Configure<AppSettings>(Configuration.GetSection(ConstantValues.AppSettings));

            // Explicitly register the settings object so IOptions not required (optional)
            services.AddSingleton(resolver =>
                resolver.GetRequiredService<IOptions<AppSettings>>().Value);


            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SquaresApp.API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SquaresApp.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
