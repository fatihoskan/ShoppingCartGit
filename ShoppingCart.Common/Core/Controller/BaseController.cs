using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Common.Core.Response;
using ShoppingCart.Common.Logging;

namespace ShoppingCart.Common.Core
{
    public abstract class BaseController : Controller
    {
        protected ILogging logger;



        protected BaseController(ILogging logger)
        {
            this.logger = logger;
        }

        protected ObjectResult OOk(object o)
        {
            return Ok(new Res(o));
        }

        protected ObjectResult OOk(object o, string mes)
        {
            return Ok(new Res(o, mes));
        }


        protected ObjectResult BBadRequest(string s)
        {
            return BadRequest(new BaseResponse(ErrorCodes.Failure, s));
        }

        protected NoContentResult NNoContent()
        {
            return NoContent();
        }

        protected ObjectResult NNotFound(string s)
        {
            return NotFound(new BaseResponse(ErrorCodes.NotFound, s));
        }

        protected ObjectResult NUnauthorized(string s)
        {
            return Unauthorized(new BaseResponse(ErrorCodes.Login, s));
        }

    }
}
