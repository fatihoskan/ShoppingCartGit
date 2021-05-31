using System;

namespace ShoppingCart.Services.Models.Request
{
    public class AddProductToCartRequest
    {
       // public Guid? CartId { get; set; }
        public Guid? SessionId { get; set; }
        public Guid? ProductId { get; set; }
        public int   Count { get; set; }
    }
}
