using Microsoft.Extensions.DependencyInjection;
using System;

namespace ShoppingCart.Common.Attributes
{
	public class SingletonDependencyAttribute : BaseDependencyAttribute
    {
        public SingletonDependencyAttribute(params Type[] serviceTypes)
            : base(ServiceLifetime.Singleton, serviceTypes)
        {
        }
    }
}
