using ShoppingCart.Common.Attributes;
using ShoppingCart.Common.Core;
using ShoppingCart.Common.Logging;
using ShoppingCart.Services.Interfaces;

namespace ShoppingCart.Services
{
    [ScopedDependency(ServiceType = typeof(IProductService))]
    public class ProductService : CoreService, IProductService
    {
        public ProductService(ILoggingHandler<ProductService> logger) : base(logger)
        {
        }
    }
}
