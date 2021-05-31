using Microsoft.Extensions.DependencyInjection;
using System;

namespace ShoppingCart.Common.Attributes
{
    public class TransientDependencyAttribute : BaseDependencyAttribute
    {
        public TransientDependencyAttribute(params Type[] serviceTypes)
            : base(ServiceLifetime.Transient, serviceTypes)
        {
        }
    }
}
