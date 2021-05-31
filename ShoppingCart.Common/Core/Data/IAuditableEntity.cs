using System;

namespace ShoppingCart.Common.Core.Data
{
    public interface IAuditableEntity
    {
        DateTime? DateCreated { get; set; }
        DateTime? DateUpdated { get; set; }
    }
}
