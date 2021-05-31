using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace ShoppingCart.UnitTests.Helper
{
    public static class RequestHelper
    {
        public static StringContent GetStringContent(object obj)
            => new StringContent(JsonConvert.SerializeObject(obj), Encoding.Default, "application/json");
    }
}
