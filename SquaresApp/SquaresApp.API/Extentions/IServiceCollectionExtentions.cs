using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SquaresApp.Application.IServices;
using SquaresApp.Application.Services;
using SquaresApp.Common.Constants;
using SquaresApp.Common.Models;
using SquaresApp.Common.SwaggerUtils;
using SquaresApp.Data.IRepositories;
using SquaresApp.Data.Repositories;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SquaresApp.API.Extentions
{
    public static class IServiceCollectionExtentions
    {
        /// <summary>
        /// add all the services and repositories dependencies in the IServiceCollection DI container
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddServicesAndRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPointService, PointService>();
            services.AddScoped<IPointRepository, PointRepository>();
            services.AddScoped<ISquaresService, SquaresService>();

            return services;
        }

        /// <summary>
        /// add JWT bearer authentication in IServiceCollection DI container
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddJWTAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
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
                    ClockSkew = TimeSpan.FromSeconds(15),//Allow 15 Sec Tolerance in expiration
                    RequireSignedTokens = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration[ConstantValues.JWTSecretPath])),
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

                            var result = JsonSerializer.SerializeToUtf8Bytes(new Response<string> { Message = ConstantValues.UnauthorizedRequestMessage });

                            await context.Response.Body.WriteAsync(result);
                        });

                        return Task.CompletedTask;
                    }
                };
            });

            return services;
        }


        /// <summary>
        /// add swagger open api docs in IServiceCollection DI container
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
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
                    Description = ConstantValues.BearerTokenDescription,
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
                c.OperationFilter<FileUploadOperation>();
                c.SchemaFilter<SwaggerSchemaFilter>(); 
                c.OperationFilter<AddHeaderOperationFilter>(ConstantValues.CorrelationIdHeader, ConstantValues.CorrelationIDHeaderDescription, false); // adds any string you like to the request headers - in this case, a correlation id

            });


            return services;
        }


        /// <summary>
        /// add distributed caching in IServiceCollection DI container
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddDistributedCaching(this IServiceCollection services, IConfiguration configuration)
        {

            var redisConnString = configuration[ConstantValues.RedisConnString];
            var redisInstanceName = $"{typeof(Startup).Namespace}-{Guid.NewGuid()}";

            if (string.IsNullOrWhiteSpace(redisConnString))
            {
                services.AddDistributedMemoryCache();
            }
            else
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = redisConnString;
                    options.InstanceName = redisInstanceName;
                });
            }


            return services;
        }

        /// <summary>
        /// add distributed caching in IServiceCollection DI container
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddAppSettings(this IServiceCollection services, IConfiguration configuration)
        {

            services.Configure<AppSettings>(configuration.GetSection(ConstantValues.AppSettings)).AddSingleton(resolver => resolver.GetRequiredService<IOptions<AppSettings>>().Value);

            return services;
        }


        /// <summary>
        /// add Wildcard cors policy in IServiceCollection DI container
        /// </summary>
        /// <param name="services"></param> 
        /// <returns></returns>
        public static IServiceCollection AddWildcardCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(ConstantValues.AllowAllOriginsCorsPolicy, builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });
            return services;
        }





    }
}
