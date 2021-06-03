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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Newtonsoft.Json;
using System.Reflection;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

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
            services.AddScoped<IPointService, PointService>();
            services.AddScoped<IPointRepository, PointRepository>();
            services.AddScoped<ISquaresService, SquaresService>();

            services.AddDbContext<SquaresAppDBContext>(options => options.UseSqlServer(Configuration.GetConnectionString(ConstantValues.DbConnString)));

            services.AddAutoMapper(config => config.AddProfile<AutoMapperProfiles>());


            // Bind the configuration using IOptions
            services.Configure<AppSettings>(Configuration.GetSection(ConstantValues.AppSettings));

            // Explicitly register the settings object so IOptions not required (optional)
            services.AddSingleton(resolver =>
                resolver.GetRequiredService<IOptions<AppSettings>>().Value);


            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
                    {
                        options.RequireHttpsMetadata = false;
                        options.SaveToken = true;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateLifetime = true,
                            RequireExpirationTime = true,
                            ClockSkew = TimeSpan.FromSeconds(15),//Allow 15 Sec Tolrance in expiration
                            RequireSignedTokens = true,
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration[ConstantValues.JWTSecretPath])),
                            ValidateIssuer = false,
                            ValidateAudience = false,
                        };
                        options.Events = new JwtBearerEvents
                        {
                            OnChallenge = context =>
                            {
                                context.Response.OnStarting(async () =>
                                {
                                    context.Response.ContentType = ConstantValues.JSONContentType;

                                    var result = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new Response<string> { Message = ConstantValues.UnauthorizedRequestMessage }));

                                    await context.Response.Body.WriteAsync(result);
                                });

                                return Task.CompletedTask;
                            }
                        };
                    });


            services.AddControllers();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(ConstantValues.V1, new OpenApiInfo
                {
                    Version = ConstantValues.V1,
                    Title = ConstantValues.ProjectTitle,
                    Description = ConstantValues.ProjectDescription,
                    Contact = new OpenApiContact
                    {
                        Name = "Shekhar Kumar Roy",
                        Email = "shekhar.roy@nagarro.com",
                        Url = new Uri("https://shekharroy.com"),
                    },
                });

                c.AddSecurityDefinition(ConstantValues.Bearer, new OpenApiSecurityScheme
                {
                    Description = ConstantValues.BearerTokenDiscription,
                    Name = ConstantValues.Authorization,
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = ConstantValues.Bearer.ToLower()
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            BearerFormat=ConstantValues.JWT,
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = ConstantValues.Bearer
                            },
                            Scheme =ConstantValues.Bearer.ToLower(),
                            Name = ConstantValues.Bearer,
                            In = ParameterLocation.Header,
                            Type=SecuritySchemeType.Http
                        },
                            new List<string>()
                        }
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);


                c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>(); // Adds "(Auth)" to the summary so that you can see which endpoints have Authorization

            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(c => c.Run(async context =>
                            { 
                                context.Response.StatusCode = StatusCodes.Status500InternalServerError; 
                                var ex = context.Features.Get<IExceptionHandlerPathFeature>().Error;
                                var response = new Response<object> { Message = ConstantValues.UnexpectedErrorMessage, Data = new { Error = new { Message = ex.InnerException?.Message ?? ex.Message, Type = ex.GetType().ToString() } } };
                                await context.Response.WriteAsJsonAsync(response);
                            }));
            }


            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.) on specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SquaresApp.API v1");
                c.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
