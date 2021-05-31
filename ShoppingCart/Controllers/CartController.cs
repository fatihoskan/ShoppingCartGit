using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Common.Attributes;
using ShoppingCart.Common.Core;
using ShoppingCart.Common.Logging;
using ShoppingCart.Services.Interfaces;
using ShoppingCart.Services.Models.Request;
using System.Threading.Tasks;

namespace ShoppingCart.Controllers
{
    [Diagnostics]
    [Route("[controller]")]
    [ApiController]
    public class CartController : BaseController
    {
        private readonly ICartService cartService;
        public CartController(ILoggingHandler<CartController> logger, ICartService cartService) : base(logger)
        {
            this.cartService = cartService;
        }

        [AllowAnonymous]
        [HttpPost("addproduct")]
        public async Task<IActionResult> AddProductToCartAsync([FromBody] AddProductToCartRequest request)
        {
            var response = await cartService.AddProductToCartAsync(request);

            return OOk(response);
        }

        [AllowAnonymous]
        [HttpGet("test")]
        public async Task<IActionResult> Get()
        {
            return OOk("hello");
        }

    }
}
