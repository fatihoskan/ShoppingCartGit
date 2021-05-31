using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.IO;
using Serilog.Context;
using ShoppingCart.Common.Attributes;
using ShoppingCart.Common.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.Common.Middlewares
{
    public class DiagnosticsMiddleware
    {
        private readonly RequestDelegate next;

        public DiagnosticsMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context, ILoggingHandler<DiagnosticsMiddleware> logger)
        {
            var executingEnpoint = context.GetEndpoint();
            if (executingEnpoint == null)
            {
                await next(context);
                return;
            }

            var diagnosticsAttributes = executingEnpoint.Metadata.OfType<DiagnosticsAttribute>();
            if (diagnosticsAttributes == null || !diagnosticsAttributes.Any())
            {
                await next(context);
                return;
            }

            var diagnosticsAttribute = diagnosticsAttributes.FirstOrDefault();
            if (diagnosticsAttribute == null)
            {
                await next(context);
                return;
            }

            var controllerActionDescriptor = executingEnpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
            if (controllerActionDescriptor == null)
            {
                await next(context);
                return;
            }

            var actionName = controllerActionDescriptor.ActionName;
            var ignoreRequestData = !string.IsNullOrEmpty(actionName) && diagnosticsAttribute.IgnoreRequestActionNames != null && diagnosticsAttribute.IgnoreRequestActionNames.Length > 0 && diagnosticsAttribute.IgnoreRequestActionNames.Contains(actionName);
            var ignoreResponseData = !string.IsNullOrEmpty(actionName) && diagnosticsAttribute.IgnoreResponseActionNames != null && diagnosticsAttribute.IgnoreResponseActionNames.Length > 0 && diagnosticsAttribute.IgnoreResponseActionNames.Contains(actionName);

            var request = context.Request;

            var path = request.Path != null ? request.Path.ToString() : string.Empty;
            path = path.ToLowerInvariant();

            var sw = Stopwatch.StartNew();

            try
            {
                request.EnableBuffering();

                if (!ignoreResponseData)
                {
                    var originalBodyStream = context.Response.Body;

                    var recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();

                    await using var responseBody = recyclableMemoryStreamManager.GetStream();
                    context.Response.Body = responseBody;

                    await next(context);

                    context.Response.Body.Seek(0, SeekOrigin.Begin);
                    var responseData = await new StreamReader(context.Response.Body).ReadToEndAsync();
                    context.Response.Body.Seek(0, SeekOrigin.Begin);

                    await responseBody.CopyToAsync(originalBodyStream);

                    var matchText = "JSON";
                    var requestIsJson = context.Request.GetTypedHeaders()
                                                        .Accept
                                                        .Any(t => t.Suffix.Value?.ToUpper() == matchText || t.SubTypeWithoutSuffix.Value?.ToUpper() == matchText);
                    var responseIsJson = string.IsNullOrEmpty(context.Response.ContentType) || context.Response.ContentType.ToUpper().Contains(matchText);
                    if (requestIsJson || responseIsJson)
                    {
                        LogContext.PushProperty("Response", responseData);
                    }
                }
                else
                {
                    await next(context);
                }

                sw.Stop();

                LogContext.PushProperty("ResponseCode", context.Response.StatusCode);

                await logger.CreateHttpLogAsync(context, sw.Elapsed, ignoreRequestData, controllerActionDescriptor);

            }
            catch (Exception exception)
            {
                sw.Stop();

                LogContext.PushProperty("ResponseCode", 500);

                await logger.CreateHttpLogAsync(context, sw.Elapsed, ignoreRequestData, controllerActionDescriptor, exception);

                throw;
            }
        }
    }
}
