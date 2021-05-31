using System.ComponentModel;

namespace ShoppingCart.Common.Enums
{
    public enum CartStatus
    {
        [Description("Active")]
        Active = 1,
        [Description("PaymentProcess")]
        PaymentProcess = 2,
        [Description("Ordered")]
        Ordered = 3,
        [Description("Passive")]
        Passive = 4
    }
}
