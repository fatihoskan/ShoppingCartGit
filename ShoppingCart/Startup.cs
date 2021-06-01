using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using ShoppingCart.Common.Extensions;
using ShoppingCart.Db.Extensions;
using ShoppingCart.Db.Persistence;
using System;

namespace ShoppingCart
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
            services.AddDbContext<ShoppingCartDbContext>(options => 
                options.UseSqlServer(
                    Configuration.GetConnectionString("ShoppingCartDbContext"),
                    x => x.MigrationsAssembly("ShoppingCart.Db")
                )
            );

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = false,
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });


            services.AddHttpContextAccessor();
            services.SetCurrentUser();
            services.AddPersistence<ShoppingCartDbContext>(Configuration);
            services.InjectAssembly("ShoppingCart.Services");

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


            services.AddControllers(options =>
            {
                options.Filters.Add(new ResponseCacheAttribute { NoStore = true, Location = ResponseCacheLocation.None });
                options.CacheProfiles.Add("NoCache", new CacheProfile { Duration = 0, NoStore = true, Location = ResponseCacheLocation.None });
            })
                .AddJsonOptions(options => options.JsonSerializerOptions.IgnoreNullValues = true)
                .AddDataAnnotationsLocalization()
                .AddViewLocalization()
                .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.EnsureMigration();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseDiagnostics();
            app.UseException();
            app.UseCors("EnableCORS");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseResponseCaching();
            app.UseEndpoints(endpoints => endpoints.MapControllers());

        }
    }
}
