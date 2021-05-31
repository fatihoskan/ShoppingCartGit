using ShoppingCart.Common.Core.Data;
using System;

namespace ShoppingCart.Db.Data
{
    public class User : AuditableEntity
    {
        public string Name { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
    }
}
