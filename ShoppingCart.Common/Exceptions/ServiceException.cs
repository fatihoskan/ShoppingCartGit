namespace ShoppingCart.Common.Exceptions
{
    public class ServiceException : BaseException
    {
        public ServiceException(string message) : base(message)
        {
        }
    }
}
