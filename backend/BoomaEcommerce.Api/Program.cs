using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Services;
using Serilog;

namespace BoomaEcommerce.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Development.json")
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true)
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom
                .Configuration(configuration)
                .CreateLogger();

            Log.Information("Starting application...");
            try
            {
                var host = CreateHostBuilder(args).Build();
                var sp = host.Services;
                var appInitializer = (AppInitializer) sp.GetService(typeof(AppInitializer));
                await appInitializer.InitializeAsync();
                await host.StartAsync();
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Failed to start application. exiting.");
            }
            finally
            {
                Log.CloseAndFlush();
            }

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
