using System;

namespace ShoppingCart.Common.Core.Data
{
    public abstract class AuditableEntity : CoreEntity, IAuditableEntity
    {
        public DateTime? DateCreated { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? DateUpdated { get; set; }

        public string UpdatedBy { get; set; }
    }
}
