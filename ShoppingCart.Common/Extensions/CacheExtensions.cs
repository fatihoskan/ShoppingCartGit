using Microsoft.Extensions.DependencyInjection;

namespace ShoppingCart.Common.Extensions
{
    public static class CacheExtensions
    {
        public static IServiceCollection AddCache(this IServiceCollection services)
        {
            services.AddDistributedMemoryCache();
            return services;
        }
    }
}
