using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using ShoppingCart.Common.Enums;
using ShoppingCart.Services.Models.Request;
using ShoppingCart.Services.Models.Response;
using ShoppingCart.UnitTests.Helper;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace ShoppingCart.UnitTests
{
    public class CartTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        public readonly HttpClient client;

        public CartTests(WebApplicationFactory<Startup> fixture)
        {
            client = fixture.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiRmF0aWggw5Zza2FuIiwiaWF0IjoxNTE2MjM5MDIyLCJ1bmlxdWVfbmFtZSI6IjY4MTMyNWQxLTc4NTgtNDNmMC05OWJiLTBlYTI5Mjk0MDQ5MiIsImVtYWlsIjoiZmF0aWhvc2thbkBnbWFpbC5jb20ifQ.SMs9LzNgYQHOsQQncrXGUHRYmfWLMsicoIauYT02ABw");
        }

        [Fact]
        public async Task AddProductToCartAsync_ProductIdNull_BadRequestMissingProductId()
        {
            //Arrange
            var request = CreateRequestForAddProductToCart(new AddProductToCartRequest
            {
                ProductId = null
            });


            //Act
            var response = await client.PostAsync(request.Url, RequestHelper.GetStringContent(request.Body));
            var res = response.GetResponseContent();


            //Assert
            Assert.IsType<HttpResponseMessage>(response);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            res.Message.Should().Be("missingproductid");
        }

        [Fact]
        public async Task AddProductToCartAsync_SessionIdNull_BadRequestSessionIdCannotBeNull()
        {
            //Arrange
            var request = CreateRequestForAddProductToCart(new AddProductToCartRequest
            {
                SessionId = null,
                ProductId = Guid.Parse("7C124CFB-5578-4232-8D44-16C20B5FBF46"),
                Count = 1
            });

            //Act
            var response = await client.PostAsync(request.Url, RequestHelper.GetStringContent(request.Body));
            var res = response.GetResponseContent();


            //Assert
            Assert.IsType<HttpResponseMessage>(response);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            res.Message.Should().Be("sessionidcannotbenull");
        }


        [Fact]
        public async Task AddProductToCartAsync_WrongProductId_BadRequestProductNotFound()
        {
            //Arrange
            var request = CreateRequestForAddProductToCart(new AddProductToCartRequest
            {
                SessionId = Guid.Parse("e56d2f23-536c-474c-b3f0-eb046fde66e0"),
                ProductId = Guid.Parse("226e67ad-5ea1-4535-b9ab-420cf182baa4"),
                Count = 1
            });


            //Act
            var response = await client.PostAsync(request.Url, RequestHelper.GetStringContent(request.Body));
            var res = response.GetResponseContent();


            //Assert
            Assert.IsType<HttpResponseMessage>(response);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            res.Message.Should().Be("productnotfound");
        }

        [Fact]
        public async Task AddProductToCartAsync_RightProductId_BadRequestInsufficientProduct()
        {
            //Arrange
            var request = CreateRequestForAddProductToCart(new AddProductToCartRequest
            {
                SessionId = Guid.Parse("e56d2f23-536c-474c-b3f0-eb046fde66e0"),
                ProductId = Guid.Parse("5D9BB3BC-84B2-45D9-9FDC-E6A4C28D541E"),
                Count = 2
            });


            //Act
            var response = await client.PostAsync(request.Url, RequestHelper.GetStringContent(request.Body));
            var res = response.GetResponseContent();


            //Assert
            Assert.IsType<HttpResponseMessage>(response);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            res.Message.Should().Be("insufficientproductcount");
        }

        [Fact]
        public async Task AddProductToCartAsync_UserId_BadRequestCartDoesnotBelongToUser()
        {
            //Arrange
            var request = CreateRequestForAddProductToCart(new AddProductToCartRequest
            {
                SessionId = Guid.Parse("e56d2f23-536c-474c-b3f0-eb046fde66e0"),
                ProductId = Guid.Parse("5D9BB3BC-84B2-45D9-9FDC-E6A4C28D541E"),
                Count = 1
            });


            //Act
            var response = await client.PostAsync(request.Url, RequestHelper.GetStringContent(request.Body));
            var res = response.GetResponseContent();


            //Assert
            Assert.IsType<HttpResponseMessage>(response);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            res.Message.Should().Be("cartisnotbelongtocurrentuser");
        }


        [Fact]
        public async Task AddProductToCartAsync_RightProductId_ActualResult()
        {
            //Arrange
            var request = CreateRequestForAddProductToCart(new AddProductToCartRequest
            {
                SessionId = Guid.Parse("5D9BB3BC-84B2-45D9-9FDC-E6A4C28D541E"),
                ProductId = Guid.Parse("7C124CFB-5578-4232-8D44-16C20B5FBF46"),
                Count = 2
            });


            //Act
            var response = await client.PostAsync(request.Url, RequestHelper.GetStringContent(request.Body));
            var res = response.GetResponseContent();
            var cartResponse = JsonConvert.DeserializeObject<CartResponse>(res.Data.ToString());
            
            
            //Assert
            Assert.IsType<HttpResponseMessage>(response);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            cartResponse.Should().BeEquivalentTo(GetExpectedItem(), options => options.Excluding(x => x.CartId));
        }

        private CartResponse GetExpectedItem()
        {
            return new CartResponse
            {
                TotalPrice = 319.98M,
                SessionId = Guid.Parse("5D9BB3BC-84B2-45D9-9FDC-E6A4C28D541E"),
                UserId = Guid.Parse("681325D1-7858-43F0-99BB-0EA292940492"),
                Status = CartStatus.Active,
                Products = new List<ProductInCartResponse>() {
                     new ProductInCartResponse{
                        Count = 2,
                        Name = "Kol saati",
                        Price = 319.98M,
                        ProductId = Guid.Parse("7C124CFB-5578-4232-8D44-16C20B5FBF46")
                     }
                }
            };
        }

        private ApiRequest CreateRequestForAddProductToCart(AddProductToCartRequest cartRequest)
        {
            return new()
            {
                Url = "/cart/addproduct",
                Body = cartRequest
            };
        }

        private class ApiRequest
        {
            public string Url { get; set; }
            public object Body { get; set; }
        }

    }
}
