using Newtonsoft.Json;
using ShoppingCart.Common.Core.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.UnitTests.Helper
{
    public static class ResponseHelper
    {
        public static Res GetResponseContent(this HttpResponseMessage response)
            =>  JsonConvert.DeserializeObject<Res>(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
    }
}
