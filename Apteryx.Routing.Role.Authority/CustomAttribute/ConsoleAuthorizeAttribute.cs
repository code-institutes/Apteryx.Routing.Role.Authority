using Apteryx.MongoDB.Driver.Extend;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Apteryx.Routing.Role.Authority
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ConsoleAuthorizeAttribute : Attribute, IAuthorizationFilter, IActionFilter
    {
        private readonly ApteryxDbContext _db;
        public ConsoleAuthorizeAttribute(ApteryxDbContext mongoDbService)
        {
            this._db = mongoDbService;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result == null)
            {
                try
                {
                    context.Result = new OkObjectResult(ApteryxResultApi.Fail(ApteryxCodes.发生未知错误, context.Exception.Message))
                    {
                        StatusCode = 200
                    };
                    context.Exception = null;
                }
                catch (Exception e)
                {
                    context.Result = new OkObjectResult(ApteryxResultApi.Fail(ApteryxCodes.发生未知错误, e.Message))
                    {
                        StatusCode = 200
                    };
                    context.Exception = null;
                }
                return;
            }
            return;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var request = context.HttpContext.Request;

            if (!context.ModelState.IsValid)
            {
                var options = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
                };
                context.Result = new OkObjectResult(ApteryxResultApi.Fail(ApteryxCodes.字段验证未通过, JsonSerializer.Serialize(context.ModelState
                    .Where(w => w.Value.Errors.FirstOrDefault() != null)
                    .Select(s => new FieldValid { Field = s.Key, Error = s.Value.Errors.Select(s1 => s1.ErrorMessage) }), options)));
                return;
            }
            return;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                var method = context.HttpContext.Request.Method;
                var template = $"/{context.ActionDescriptor.AttributeRouteInfo.Template}";
                var accountId = context.HttpContext.User.Identity.Name;

                var systemAccount = _db.SystemAccounts.FindOne(f => f.Id == accountId);
                if (systemAccount.IsSuper)
                    return;

                var route = _db.Routes.FindOne(f => f.Method == method && f.Path == template);
                if (route != null)
                {
                    var roleRoute = _db.Roles.FindOne(f => f.Id == systemAccount.RoleId && f.RouteIds.Contains(route.Id));
                    if (roleRoute != null)
                    {
                        return;
                    }
                    else
                    {
                        var role = _db.Roles.FindOne(systemAccount.RoleId);
                        context.Result = new BadRequestObjectResult(ApteryxResultApi.Fail(ApteryxCodes.权限不足, $"角色：“{role.Name}”无权访问当前路由！")) { StatusCode = 200 };
                        return;
                    }
                }
                else
                {
                    context.Result = new BadRequestObjectResult(ApteryxResultApi.Fail(ApteryxCodes.路由不存在, $"路由：“{template}”在数据库中未找到！")) { StatusCode = 200 };
                }
            }
            return;
        }
    }
}
