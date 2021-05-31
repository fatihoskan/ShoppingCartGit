using Microsoft.AspNetCore.Builder;
using ShoppingCart.Common.Middlewares;

namespace ShoppingCart.Common.Extensions
{
    public static class DiagnosticsExtensions
    {
        public static IApplicationBuilder UseDiagnostics(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DiagnosticsMiddleware>();
        }
    }
}
