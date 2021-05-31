using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog.Context;
using ShoppingCart.Common.Attributes;
using ShoppingCart.Common.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Common.Logging
{
    [ScopedDependency(ServiceType = typeof(ILoggingHandler<>))]
    public class LoggingHandler<T> : LoggingHandlerBase, ILoggingHandler<T>
    {
        private readonly ILogger<T> logger;
        private readonly string appName;
        private readonly string environmentName;

        public LoggingHandler(IConfiguration config, ILogger<T> logger)
            :base(config)
        {
            this.logger = logger;
            if (config != null)
            {
                appName = config.GetAppName();
                environmentName = StartupExtensions.Environment.ToDescriptionString();
            }
            else
            {
                appName = "None";
                environmentName = "None";
            }
        }

        public void Debug(string message, IDictionary<string, string> properties = null)
        {
            if (properties != null && properties.Count > 0)
            {
                logger.LogDebug("{Environment} - {AppName} - {Message} - {Properties}", environmentName, appName, message, properties);
            }
            else
            {
                logger.LogDebug("{Environment} - {AppName} - {Message}", environmentName, appName, message);
            }
        }

        public void Warning(string message, IDictionary<string, string> properties = null)
        {
            if (properties != null && properties.Count > 0)
            {
                logger.LogWarning("{Environment} - {AppName} - {Message} - {Properties}", environmentName, appName, message, properties);
            }
            else
            {
                logger.LogWarning("{Environment} - {AppName} - {Message}", environmentName, appName, message);
            }
        }

        public void Information(string message, IDictionary<string, string> properties = null)
        {
            if (properties != null && properties.Count > 0)
            {
                logger.LogInformation("{Environment} - {AppName} - {Message} - {Properties}", environmentName, appName, message, properties);
            }
            else
            {
                logger.LogInformation("{Environment} - {AppName} - {Message}", environmentName, appName, message);
            }
        }

        public void Error(string message, Exception exception = null, IDictionary<string, string> properties = null)
        {
            if (properties != null && properties.Count > 0)
            {
                logger.LogError(exception, "{Environment} - {AppName} - {Message} - {Properties}", environmentName, appName, message, properties);
            }
            else
            {
                logger.LogError(exception, "{Environment} - {AppName} - {Message}", environmentName, appName, message);
            }
        }

        public async Task FatalAsync(HttpContext context, Exception exception)
        {
            LogContext.PushProperty("ResponseCode", 500);
            logger.LogCritical(exception, "{Environment} - {AppName} - {Message}", environmentName, appName, exception.Message);

            //eğer ihtiyaç duyulursa verilecek bir slack adresine log iletilebilir...
            // await ForwardAsync(context, exception);
        }

        public string MaskSensitiveData(string data)
        {
            if (string.IsNullOrEmpty(data))
                return string.Empty;
            var token = JToken.Parse(data);
            var maskPaths = new string[]
            {
                "$..Password",
                "$..OldPassword",
                "$..NewPassword",
                "$..ConfirmPassword",
                "$..PasswordAgain",
                "$..NewPasswordAgain",
                "$..password",
                "$..oldPassword",
                "$..newPassword",
                "$..confirmPassword",
                "$..passwordAgain",
                "$..newPasswordAgain"
            };
            MaskMatchingValues(token, maskPaths);
            return token.ToString(Formatting.Indented);
        }

        public async Task CreateHttpLogAsync(HttpContext context, TimeSpan timeElapsed, bool ignoreRequestData, ControllerActionDescriptor controllerActionDescriptor, Exception exception = null)
        {
            var request = context.Request;
            var controllerName = controllerActionDescriptor?.ControllerName;
            var actionName = controllerActionDescriptor?.ActionName;

            try
            {
                var stringElapsed = string.Format("{0:00}:{1:00}:{2:00}.{3:00}", timeElapsed.Hours, timeElapsed.Minutes, timeElapsed.Seconds, timeElapsed.Milliseconds / 10);
                LogContext.PushProperty("ElapsedTime", stringElapsed);

                LogContext.PushProperty("ActionName", actionName);
                LogContext.PushProperty("ControllerName", controllerName);

                string requestData = null;

                if (!ignoreRequestData)
                {
                    if (request != null)
                    {
                        if (request.Query != null && request.Query.Any())
                        {
                            var queries = request.Query.ToDictionary(v => v.Key, v => v.Value.ToString());
                            requestData = JsonConvert.SerializeObject(queries);
                        }
                        else if (request.HasFormContentType)
                        {
                            var forms = request.Form.ToDictionary(v => v.Key, v => v.Value.ToString());
                            requestData = JsonConvert.SerializeObject(forms);
                        }
                        else
                        {
                            if (request.Body.CanSeek)
                            {
                                request.Body.Seek(0, SeekOrigin.Begin);
                                using var reader = new StreamReader(request.Body, encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 8192, leaveOpen: true);
                                requestData = await reader.ReadToEndAsync();
                                request.Body.Seek(0, SeekOrigin.Begin);
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(requestData))
                {
                    LogContext.PushProperty("Request", MaskSensitiveData(requestData));
                }
            }
            catch
            {
            }

            if (exception != null)
            {
                Error("Diagnostic Error", exception);
            }
            else
            {
                if (context.Response.StatusCode == 200)
                {
                    Information($"{controllerName} - {actionName} Requested.");
                }
                else
                {
                    Warning($"{controllerName} - {actionName} Requested.");
                }
            }
        }

        private static void MaskMatchingValues(JToken token, IEnumerable<string> jsonPaths)
        {
            foreach (var path in jsonPaths)
                foreach (var match in token.SelectTokens(path))
                    match.Replace(new JValue("******"));
        }
    }
}
