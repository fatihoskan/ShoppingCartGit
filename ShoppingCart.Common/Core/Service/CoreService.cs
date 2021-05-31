using ShoppingCart.Common.Logging;

namespace ShoppingCart.Common.Core
{
    public abstract class CoreService
    {
        protected ILogging logger;

        public CoreService(ILogging logger)
        {
            this.logger = logger;
        }
    }
}
