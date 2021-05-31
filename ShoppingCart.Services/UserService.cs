using ShoppingCart.Common.Attributes;
using ShoppingCart.Common.Core;
using ShoppingCart.Common.Logging;
using ShoppingCart.Services.Interfaces;

namespace ShoppingCart.Services
{
    [ScopedDependency(ServiceType = typeof(IUserService))]
    public class UserService : CoreService, IUserService
    {
        public UserService(ILoggingHandler<UserService> logger) : base(logger)
        {
        }
    }
}
