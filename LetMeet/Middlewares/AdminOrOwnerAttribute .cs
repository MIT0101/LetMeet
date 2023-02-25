using LetMeet.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Org.BouncyCastle.Ocsp;
using System.Security.Claims;

namespace LetMeet.Middlewares
{
    public class OwnerOrInRoleGuidAttribute : ActionFilterAttribute
    {
        //usage
        //[Authorize]
        //[AdminOrOwnerGuid(queryIdName: "id")]
        //public async Task<IActionResult> EditProfile(Guid id)
        //{ 
        //working
        //}
        private readonly string _idFieldName;
        private readonly string _role;

        private ILogger<OwnerOrInRoleGuidAttribute> _logger;
        public OwnerOrInRoleGuidAttribute(string IdFieldName,string Role)
        {
            _idFieldName = IdFieldName;
            _role = Role;
            if (string.IsNullOrWhiteSpace(_role))
            {
                throw new ArgumentNullException(nameof(Role));
            }
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {

            //Can get any service service 
            //context.HttpContext.RequestServices.GetRequiredService<IMyRepository>();

            _logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<OwnerOrInRoleGuidAttribute>>();
            

            Guid requstedId;
            Guid userInfoId;

            if (!Guid.TryParse(context.HttpContext.Request.RouteValues[_idFieldName]?.ToString(), out requstedId))
            {
                _logger.LogError("Can Not Get Id Field");
                context.Result = new StatusCodeResult(StatusCodes.Status406NotAcceptable);
                return;
            }

            if (!Guid.TryParse(context.HttpContext.User.FindFirstValue(ClaimsNameHelper.UserInfoId), out userInfoId))
            {
                _logger.LogError("Can Not Get Current User Id");

                context.Result = new StatusCodeResult(StatusCodes.Status406NotAcceptable);
                return;
            }

            if (!context.HttpContext.User.IsInRole(_role) && !userInfoId.Equals(requstedId))
            {
                _logger.LogWarning("Unauthorized User Data Request");

                context.Result = new UnauthorizedResult();
                return;
            }

            base.OnActionExecuting(context);

        }
    }
}
