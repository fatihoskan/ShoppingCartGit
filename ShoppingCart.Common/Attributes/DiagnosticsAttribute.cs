using System;

namespace ShoppingCart.Common.Attributes
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DiagnosticsAttribute : Attribute
    {
        public string[] IgnoreRequestActionNames { get; set; }

        public string[] IgnoreResponseActionNames { get; set; }

        public DiagnosticsAttribute(params string[] ignoreRequestActionNames)
        {
            IgnoreRequestActionNames = ignoreRequestActionNames;
        }

        public DiagnosticsAttribute(string[] ignoreRequestActionNames, params string[] ignoreResponseActionNames)
        {
            IgnoreRequestActionNames = ignoreRequestActionNames;
            IgnoreResponseActionNames = ignoreResponseActionNames;
        }
    }
}
