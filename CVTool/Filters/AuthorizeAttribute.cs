using CVTool.Data.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using CVTool.Services.JwtUtils;
using CVTool.Services.UserService;

namespace CVTool.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous)
                return;

            var jwtUtils =
            context.HttpContext.RequestServices.GetService(typeof(IJwtUtils))
                as IJwtUtils;

            var userService =
            context.HttpContext.RequestServices.GetService(typeof(IUserService))
                as IUserService;


            var token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = jwtUtils.ValidateJwtToken(token);

            if (userId != null)
            {
                context.HttpContext.Items["User"] = userService.GetById(userId.Value);
            }

            var user = (User)context.HttpContext.Items["User"];
            if (user == null)
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
        }
    }
}
