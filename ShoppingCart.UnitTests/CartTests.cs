using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using ShoppingCart.Common.Core.Response;
using ShoppingCart.Common.Logging;
using ShoppingCart.Controllers;
using ShoppingCart.Services.Interfaces;
using ShoppingCart.Services.Models.Request;
using ShoppingCart.UnitTests.Helper;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Xunit;

namespace ShoppingCart.UnitTests
{
    public class CartTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        public readonly HttpClient client;

        public CartTests(WebApplicationFactory<Startup> fixture)
        {
            client = fixture.CreateClient();
        }

        [Fact]
        public async Task AddProductToCartAsync_MissingProductId_BadRequestMissingProductId()
        {
            //Arrange
            var request = new
            {
                Url = "/cart/addproduct",
                Body = new AddProductToCartRequest
                {
                    ProductId = null
                }
            };

            //Act
            var response = await  client.PostAsync(request.Url, RequestHelper.GetStringContent(request.Body));

            //Assert
            Assert.IsType<HttpResponseMessage>(response);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            //response.RequestMessage.Should.Be("");
        }
    }
}
