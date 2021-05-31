using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ShoppingCart.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class BaseDependencyAttribute : Attribute
    {
        public ServiceLifetime DependencyType { get; set; }

        public Type ServiceType { get; set; }

        public Type[] ServiceTypes { get; set; }

        protected BaseDependencyAttribute(ServiceLifetime dependencyType, params Type[] serviceTypes)
        {
            DependencyType = dependencyType;
            ServiceTypes = serviceTypes;
        }

        public List<ServiceDescriptor> ServiceDescriptors(TypeInfo type)
        {
            var serviceDescriptors = new List<ServiceDescriptor>();
            if (ServiceTypes != null && ServiceTypes.Length > 0)
            {
                foreach (var st in ServiceTypes)
                    serviceDescriptors.Add(new ServiceDescriptor(st, type.AsType(), DependencyType));
            }
            var serviceType = ServiceType ?? type.AsType();
            serviceDescriptors.Add(new ServiceDescriptor(serviceType, type.AsType(), DependencyType));
            return serviceDescriptors;
        }
    }
}
