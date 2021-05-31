using Newtonsoft.Json;

namespace ShoppingCart.Common.Core.Response
{
    public class BaseResponse
    {
        public int Code { get; set; } = ErrorCodes.Success;
        public string Message { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public BaseResponse()
        {
        }
        public BaseResponse(int code, string message)
        {
            Code = code;
            Message = message;
        }
    }

    public class BaseResponse<T> : BaseResponse
    {
        public T Data { get; set; }

        public BaseResponse()
        {
        }

        public BaseResponse(T data)
        {
            Data = data;
        }
    }

    public class Res : BaseResponse
    {
        public object Data { get; set; }
        public Res()
        {
        }

        public Res(object data)
        {
            Data = data;
        }

        public Res(object data, string message)
        {
            Data = data;
            Message = message;
        }
    }

    public class ErrorCodes
    {

        public const int Success = 0;
        public const int Failure = 1;
        public const int NotFound = 2;
        public const int Login = 3;
    }
}
