using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using ShoppingCart.Common.Extensions;
using System;

namespace ShoppingCart
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = StartupExtensions.GetConfiguration(args, Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));
            var appName = config.GetAppName();
            Log.Logger = new LoggerConfiguration().RegisterLog(config.GetLogProvider(), appName);

            try
            {
                Log.Information($"{appName} Starting...");

                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, $"{appName} Failed!!!");
            }
            finally
            {
                Log.Information($"{appName} Closing...");
                Log.CloseAndFlush();
            }
        }


        public static IHostBuilder CreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args)
               .ConfigureWebHostDefaults(webBuilder =>
               {
                   webBuilder.UseStartup<Startup>();
               });

    }
}
