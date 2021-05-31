using ShoppingCart.Common.Enums;
using System;
using System.Collections.Generic;

namespace ShoppingCart.Services.Models.Response
{
    public class CartResponse
    {
        public Guid? CartId { get; set; }
        public Guid? SessionId { get; set; }
        public Guid? UserId { get; set; }
        public decimal TotalPrice { get; set; }
        public CartStatus Status { get; set; }
        public List<ProductInCartResponse> Products { get; set; }
    }
}
