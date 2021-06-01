using Microsoft.EntityFrameworkCore;
using ShoppingCart.Common.Attributes;
using ShoppingCart.Common.Core;
using ShoppingCart.Common.Core.Data;
using ShoppingCart.Common.Enums;
using ShoppingCart.Common.Exceptions;
using ShoppingCart.Common.Logging;
using ShoppingCart.Db.Data;
using ShoppingCart.Db.Persistence;
using ShoppingCart.Services.Interfaces;
using ShoppingCart.Services.Models.Request;
using ShoppingCart.Services.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.Services
{
    [ScopedDependency(ServiceType = typeof(ICartService))]
    public class CartService : CoreService, ICartService
    {
        private readonly ShoppingCartDbContext dbContext;
        private readonly CurrentUser currentUser;
        public CartService(ILoggingHandler<CartService> logger, ShoppingCartDbContext dbContext, CurrentUser currentUser) : base(logger)
        {
            this.dbContext = dbContext;
            this.currentUser = currentUser;
        }

        public async Task<CartResponse> AddProductToCartAsync(AddProductToCartRequest request)
        {
            if (request.ProductId == null || request.ProductId == Guid.Empty)
            {
                logger.Information($"missing product id");
                throw new ServiceException($"missingproductid");
            }

            if (request.SessionId == null || request.SessionId == Guid.Empty)
            {
                logger.Information($"session id cannot be null");
                throw new ServiceException("sessionidcannotbenull");
            }


            var product = await dbContext.Products.SingleOrDefaultAsync(x => x.Id == request.ProductId);

            if (product == null)
            {
                logger.Information($"product not found with the given id. productid : {request.ProductId}");
                throw new ServiceException($"productnotfound");
            }

            if (product.Count < request.Count)
            {
                logger.Information($"insufficient product count. product.Count : {request.ProductId} - requested count : {request.Count}");
                throw new ServiceException($"insufficientproductcount");
            }

            Cart cart = await dbContext.Carts.SingleOrDefaultAsync(x => x.SessionId == request.SessionId);

            if (cart != null)
            {
                if (currentUser.Id != Guid.Empty)
                {
                    if (cart.UserId != null && cart.UserId != Guid.Empty && cart.UserId != currentUser.Id)
                    {
                        logger.Information($"cart is not belong to current user. current user : {currentUser.Id}");
                        throw new ServiceException($"cartisnotbelongtocurrentuser");
                    }
                }
            }

            if (cart == null)
            {
                cart = new Cart
                {
                    SessionId = request.SessionId ?? Guid.Empty,
                    Status = CartStatus.Active,
                    UserId = currentUser.Id
                };

                await dbContext.Carts.AddAsync(cart);
                await dbContext.SaveChangesAsync();
            }

            CartProduct cartProduct = null;

            var cartProducts = await dbContext.CartProducts.Where(x => x.CartId == cart.Id).ToListAsync();

            if (cartProducts.Any())
            {
                var selectedItem = cartProducts.SingleOrDefault(x => x.ProductId == product.Id);

                if (selectedItem != null)
                {
                    selectedItem.ItemCount += request.Count;
                    dbContext.CartProducts.Update(selectedItem);
                    await dbContext.SaveChangesAsync();
                }
                else {
                    cartProduct = new()
                    {
                        CartId = cart.Id,
                        ItemCount = request.Count,
                        ProductId = product.Id
                    };
                }
            }
            else {
                cartProduct = new()
                {
                    CartId = cart.Id,
                    ItemCount = request.Count,
                    ProductId = product.Id
                };
            }

            if (cartProduct != null)
            {
                await dbContext.CartProducts.AddAsync(cartProduct);
                await dbContext.SaveChangesAsync();
                cartProducts.Add(cartProduct);
            }

            var productIdArray = cartProducts.Select(x => x.ProductId).ToArray();

            var products =  dbContext.Products.Where(x => productIdArray.Contains(x.Id)).ToList();

            List<ProductInCartResponse> productsResponse = products.Select(x => new ProductInCartResponse {
                ProductId = x.Id,
                Name = x.Name,
                Count = cartProducts.SingleOrDefault(y => y.ProductId == x.Id).ItemCount,
                Price = cartProducts.SingleOrDefault(y => y.ProductId == x.Id).ItemCount * x.Price
            }).ToList();


            var totalPrice = productsResponse.Sum(x => x.Price);

            cart.TotalPrice = totalPrice;
            dbContext.Carts.Update(cart);
            await dbContext.SaveChangesAsync();

            return new CartResponse()
            {
                CartId = cart.Id,
                SessionId = request.SessionId,
                UserId = currentUser.Id,
                Status = cart.Status,
                TotalPrice = totalPrice,
                Products = productsResponse
            };
        }
    }
}
