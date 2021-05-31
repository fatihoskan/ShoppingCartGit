using ShoppingCart.Common.Core.Data;
using ShoppingCart.Common.Enums;
using System;

namespace ShoppingCart.Db.Data
{
    public class Cart : AuditableEntity
    {
        public Guid SessionId { get; set; }
        public Guid? UserId { get; set; }
        public decimal TotalPrice { get; set; }
        public CartStatus Status { get; set; }
    }
}
