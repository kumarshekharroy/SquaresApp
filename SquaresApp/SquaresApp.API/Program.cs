using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Context;
using SquaresApp.Common.Constants;
using SquaresApp.Data.Context;
using System;

namespace SquaresApp.API
{
    public static class Program
    {
        public static void Main(string[] args)
        { 
            Environment.SetEnvironmentVariable("BASEDIRFORLOG", AppContext.BaseDirectory);

            Log.Logger = new LoggerConfiguration().CreateLogger();

            using (LogContext.PushProperty(ConstantValues.SourceContext, typeof(Program).FullName))
            {
                try
                { 
                    var host = CreateHostBuilder(args).Build();

                    Log.Logger.Information("Squares Application Starting.");

                    using var scope = host.Services.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<SquaresAppDBContext>();
                    context.Database.Migrate(); // apply all migrations   

                    host.Run();
                }
                catch (Exception ex)
                {
                    Log.Logger.Fatal(ex, "Squares Application failed to start.");
                }
                finally
                {
                    Log.CloseAndFlush();
                }
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseSerilog((hostContext, loggerConfiguration) =>
            {
                loggerConfiguration.ReadFrom.Configuration(hostContext.Configuration);
            })
            .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
