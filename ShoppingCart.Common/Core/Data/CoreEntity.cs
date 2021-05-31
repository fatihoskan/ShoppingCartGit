using System;
using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.Common.Core.Data
{
    public class CoreEntity
    {
        [Key]
        public Guid Id { get; set; }
    }
}
