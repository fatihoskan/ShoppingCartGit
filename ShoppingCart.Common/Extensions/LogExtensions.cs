using Destructurama;
using Serilog;
using Serilog.Events;

namespace ShoppingCart.Common.Extensions
{
    public static class LogExtensions
    {
        public static ILogger RegisterLog(this LoggerConfiguration loggerConfiguration, string logProvider, string appName)
        {
            return loggerConfiguration
               .WriteTo.Seq(logProvider)
               .MinimumLevel.Information()
               .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
               .MinimumLevel.Override("Microsoft.Hosting", LogEventLevel.Warning)
               .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
               .MinimumLevel.Override("Microsoft.AspNetCore.Session", LogEventLevel.Warning)
               .MinimumLevel.Override("Microsoft.AspNetCore.Session.SessionMiddleware", LogEventLevel.Warning)
               .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
               .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Query", LogEventLevel.Warning)
               .Enrich.WithProperty("AppName", appName)
               .Enrich.WithProperty("Environment", StartupExtensions.Environment.ToDescriptionString())
               .Enrich.FromLogContext()
               .WriteTo.Console()
               .Destructure.ToMaximumCollectionCount(10)
               .Destructure.ToMaximumStringLength(10000)
               .Destructure.ToMaximumDepth(5)
               .Destructure.UsingAttributes()
               .CreateLogger();
        }
    }
}