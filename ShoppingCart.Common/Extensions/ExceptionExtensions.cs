using Microsoft.AspNetCore.Builder;
using ShoppingCart.Common.Middlewares;

namespace ShoppingCart.Common.Extensions
{
    public static class ExceptionExtensions
    {
        public static IApplicationBuilder UseException(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
