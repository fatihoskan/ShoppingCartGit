using ShoppingCart.Services.Models.Request;
using ShoppingCart.Services.Models.Response;
using System.Threading.Tasks;

namespace ShoppingCart.Services.Interfaces
{
    public interface ICartService
    {
        Task<CartResponse> AddProductToCartAsync(AddProductToCartRequest request);
    }
}
