using System.ComponentModel;

namespace ShoppingCart.Common.Enums
{
    public enum EnvironmentTypes
    {
        [Description("Local")]
        Local = 1,
        [Description("Debug")]
        Debug = 2,
        [Description("Development")]
        Development = 3,
        [Description("Stage")]
        Stage = 4,
        [Description("Production")]
        Production = 5
    }
}
