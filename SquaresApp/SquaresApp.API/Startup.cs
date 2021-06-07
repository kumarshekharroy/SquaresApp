using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using SquaresApp.API.Middlewares;
using SquaresApp.Application.IServices;
using SquaresApp.Application.Profiles;
using SquaresApp.Application.Services;
using SquaresApp.Common.Constants;
using SquaresApp.Common.Models;
using SquaresApp.Common.SwaggerUtils;
using SquaresApp.Data.Context;
using SquaresApp.Data.Repositories;
using SquaresApp.Domain.IRepositories;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

            services.Configure<AppSettings>(Configuration.GetSection(ConstantValues.AppSettings)).AddSingleton(resolver => resolver.GetRequiredService<IOptions<AppSettings>>().Value);

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

            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                options.JsonSerializerOptions.IgnoreNullValues = true;
            });

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
                c.OperationFilter<FileUploadOperation>();
                c.SchemaFilter<SwaggerSchemaFilter>();

            });

            var redisConnString = Configuration[ConstantValues.RedisConnString];
            var redisInstanceName = $"{this.GetType().Namespace}-{Guid.NewGuid().ToString()}";

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

            services.AddCors(options =>
            {
                options.AddPolicy(ConstantValues.AllowAllOriginsCorsPolicy, builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger().UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SquaresApp.API v1");
                c.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();

            app.UseCors(ConstantValues.AllowAllOriginsCorsPolicy);

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseCustomCaching();

            app.UseExceptionHandler(c => c.Run(async context =>
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                var ex = context.Features.Get<IExceptionHandlerPathFeature>().Error;
                var response = new Response<object> { Message = ConstantValues.UnexpectedErrorMessage, Data = new { Error = new { Message = ex.InnerException?.Message ?? ex.Message, Type = ex.GetType().ToString() } } };
                await context.Response.WriteAsJsonAsync(response);
            }));

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
