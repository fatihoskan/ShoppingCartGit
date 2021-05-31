using ShoppingCart.Common.Core.Data;

namespace ShoppingCart.Db.Data
{
    public class Product : AuditableEntity
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Count { get; set; }
    }
}
