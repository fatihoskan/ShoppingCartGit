using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ShoppingCart.Common.Core.Response;
using ShoppingCart.Common.Exceptions;
using ShoppingCart.Common.Logging;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Common.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate next;
        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context, ILoggingHandler<ExceptionHandlingMiddleware> logger)
        {
            try
            {
                await next(context);
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(context, exception, logger);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception, ILoggingHandler<ExceptionHandlingMiddleware> logger)
        {
            context.Response.ContentType = "application/json";
            var serializeOptions = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Include,
                Formatting = Formatting.None,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            };
            if (exception is ServiceException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                logger.Warning($"ServiceException - Message: {exception.Message}");
                var serviceExceptionResponse = new Res()
                {
                    Code = ErrorCodes.Failure,
                    Message = exception.Message,
                    Data = null
                };
                await context.Response.WriteAsync(JsonConvert.SerializeObject(serviceExceptionResponse, serializeOptions), Encoding.UTF8);
                return;
            }
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            logger.Fatal(context, exception);
            var exceptionResponse = new BaseResponse()
            {
                Code = ErrorCodes.Failure,
                Message = "UnexpectedError"
            };
            await context.Response.WriteAsync(JsonConvert.SerializeObject(exceptionResponse, serializeOptions), Encoding.UTF8);
        }
    }
}
