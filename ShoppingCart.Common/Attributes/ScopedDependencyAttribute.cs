using Microsoft.Extensions.DependencyInjection;
using System;

namespace ShoppingCart.Common.Attributes
{
	public class ScopedDependencyAttribute : BaseDependencyAttribute
    {
        public ScopedDependencyAttribute(params Type[] serviceTypes)
            : base(ServiceLifetime.Scoped, serviceTypes)
        {
        }
    }
}
