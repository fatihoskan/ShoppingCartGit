using System;

namespace ShoppingCart.Common.Extensions
{
    public static class ErrorExtensions
    {
        public static string GetExceptionMessage(this Exception ex)
        {
            var message = "";
            while (ex != null)
            {
                message += ex.Message + "\r\n";
                ex = ex.InnerException;
            }
            return message;
        }
    }
}
