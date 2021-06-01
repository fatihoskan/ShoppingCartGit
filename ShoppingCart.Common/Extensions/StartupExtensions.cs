using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ShoppingCart.Common.Attributes;
using ShoppingCart.Common.Enums;
using System;
using System.Diagnostics;
using System.Reflection;
using Serilog;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using ShoppingCart.Common.Core.Data;

namespace ShoppingCart.Common.Extensions
{
    public static class StartupExtensions
    {
        public static string AppName { get; internal set; }

        public static EnvironmentTypes Environment { get; internal set; }

        public static IHostBuilder GetHostBuilder<T>(this IConfiguration config, string[] args) where T : class
        {
            var hostBuilder = Host.CreateDefaultBuilder(args)
             .ConfigureAppConfiguration((builder) =>
             {
                 builder.Sources.Clear();
                 builder.AddConfiguration(config);
             })
             .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<T>())
             .ConfigureLogging(config => config.ClearProviders())
             .UseSerilog();
            return hostBuilder;
        }

        public static IConfiguration GetConfiguration(string[] args, string environmentName)
        {
            Environment = GetEnvironmentType(environmentName);
            var fileName = Environment != EnvironmentTypes.Debug && Environment != EnvironmentTypes.Local ? $".{Environment.ToDescriptionString()}" : string.Empty;
            IConfiguration config = new ConfigurationBuilder()
                                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                                    .AddJsonFile($"appsettings{fileName}.json", optional: true, reloadOnChange: true)
                                    .AddCommandLine(args)
                                    .AddEnvironmentVariables()
                                    .Build();
            return config;
        }

        public static EnvironmentTypes GetEnvironmentType(string environmentName)
        {
            if (Debugger.IsAttached)
                return EnvironmentTypes.Debug;
            if (environmentName == "Development")
                return EnvironmentTypes.Development;
            if (environmentName == "Stage")
                return EnvironmentTypes.Stage;
            if (environmentName == "Production")
                return EnvironmentTypes.Production;
            return EnvironmentTypes.Local;
        }

        public static IServiceCollection InjectAssembly(this IServiceCollection services, string extraAssembly = null)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    var dependencyAttributes = type.GetCustomAttributes<BaseDependencyAttribute>();
                    foreach (var dependencyAttribute in dependencyAttributes)
                    {
                        var serviceDescriptors = dependencyAttribute.ServiceDescriptors(type.GetTypeInfo());
                        foreach (var serviceDescriptor in serviceDescriptors)
                            services.Add(serviceDescriptor);
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(extraAssembly))
            {
                var currentAssembly = Assembly.Load(new AssemblyName(extraAssembly));
                foreach (var type in currentAssembly.ExportedTypes)
                {
                    var dependencyAttributes = type.GetCustomAttributes<BaseDependencyAttribute>();
                    foreach (var dependencyAttribute in dependencyAttributes)
                    {
                        var serviceDescriptors = dependencyAttribute.ServiceDescriptors(type.GetTypeInfo());
                        foreach (var serviceDescriptor in serviceDescriptors)
                            services.Add(serviceDescriptor);
                    }
                }
            }

            return services;
        }


        public static string GetAppName(this IConfiguration config)
        {
            AppName = config["AppName"];
            return AppName;
        }

        public static string GetLogProvider(this IConfiguration config)
        {
            return config["LogProvider"];
        }

        
        public static IServiceCollection SetCurrentUser(this IServiceCollection services)
        {
            _ = services.AddScoped((e) =>
              {
                  try
                  {
                      var context = (e.GetService(typeof(IHttpContextAccessor)) as IHttpContextAccessor)?.HttpContext;
                      if (context == null)
                          return null;
                      var userId = Guid.Empty;
                      string userEmail = null;

                      if (context.Request.Headers.ContainsKey("Authorization"))
                      {
                          var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                          var tokenHandler = new JwtSecurityTokenHandler();
                          var decryptedToken = tokenHandler.ReadJwtToken(token);
                          var id = decryptedToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;
                          _ = Guid.TryParse(id, out userId);
                          var email = decryptedToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
                          userEmail = email;
                      }

                      else if (context.User != null)
                      {
                          var userName = context != null && context.User != null && context.User.Claims != null ? context.User.Claims.FirstOrDefault(x => x.Type == "UserName") : null;
                          var id = context != null && context.User != null && context.User.Claims != null ? context.User.Claims.FirstOrDefault(x => x.Type == "Id") : null;

                          if (id != null)
                              _ = Guid.TryParse(id.Value.ToString(), out userId);

                          if (userName != null)
                              userEmail = userName.Value.ToString();
                      }

                      if (userId != Guid.Empty && !string.IsNullOrEmpty(userEmail))
                      {
                          return new CurrentUser
                          {
                              Id = userId,
                              Username = userEmail
                          };
                      }
                  }
                  catch
                  {
                  }
                  return null;
              });
            return services;
        }


        public static IServiceCollection AddPersistence<TContext>(this IServiceCollection services, IConfiguration configuration) where TContext : DbContext
        {
            services.AddDbContext<TContext>(options =>
            {
                if (Debugger.IsAttached)
                    options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
                options.UseSqlServer(configuration.GetConnectionString("Default"), (dbOptions) =>
                {
                    dbOptions.EnableRetryOnFailure(10, TimeSpan.FromSeconds(10), null);
                    dbOptions.CommandTimeout(300);
                });
            }, ServiceLifetime.Transient, ServiceLifetime.Transient);
            return services;
        }
    }
}
