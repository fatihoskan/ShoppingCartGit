using ShoppingCart.Common.Core.Data;
using System;

namespace ShoppingCart.Db.Data
{
    public class CartProduct : AuditableEntity 
    {
        public Guid CartId { get; set; }
        public Guid ProductId { get; set; }
        public int ItemCount { get; set; }
    }
}
