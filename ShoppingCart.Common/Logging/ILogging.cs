using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace ShoppingCart.Common.Logging
{
    public interface ILogging
    {
        void Debug(string message, IDictionary<string, string> properties = null);

        void Warning(string message, IDictionary<string, string> properties = null);

        void Information(string message, IDictionary<string, string> properties = null);

        void Error(string message, Exception exception = null, IDictionary<string, string> properties = null);

        Task FatalAsync(HttpContext context, Exception exception);

        Task CreateHttpLogAsync(HttpContext context, TimeSpan timeElapsed, bool ignoreRequestData, ControllerActionDescriptor controllerActionDescriptor, Exception exception = null);

        string MaskSensitiveData(string data);
    }
}
