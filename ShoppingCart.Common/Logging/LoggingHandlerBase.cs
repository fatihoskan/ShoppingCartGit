using Microsoft.Extensions.Configuration;

namespace ShoppingCart.Common.Logging
{
    public abstract class LoggingHandlerBase
    {
        protected IConfiguration config;
        public LoggingHandlerBase(IConfiguration config)
        {
            this.config = config;
        }
    }
}