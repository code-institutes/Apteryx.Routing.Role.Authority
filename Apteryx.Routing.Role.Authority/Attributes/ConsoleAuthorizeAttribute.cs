using apteryx.common.extend.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MongoDB.Driver;
using Swashbuckle.AspNetCore.Annotations;
using System.Data;
using System.Security.Claims;
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
                    context.Result = new OkObjectResult(ApteryxResultApi.Fail(ApteryxCodes.发生未知错误, context.Exception?.Message))
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
            }

            try
            {
                var resultValue = ((ObjectResult)context.Result).Value;
                var response = new Response()
                {
                    StatusCode = context.HttpContext.Response.StatusCode,
                    Result = resultValue.ToJson(),
                    Type = resultValue?.GetType().FullName
                };

                var traceIdentifier = context.HttpContext.TraceIdentifier;
                _db.ApteryxCallLog.Commands.UpdateOne(u => u.TraceIdentifier == context.HttpContext.TraceIdentifier, Builders<CallLog>.Update.Set(s => s.Response, response));
                return;
            }
            catch { }
            return;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var actDesc = (Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)context.ActionDescriptor;
            var httpContext = context.HttpContext;
            var request = httpContext.Request;
            var method = request.Method;

            var requestInfo = new Request()
            {
                ContentType = request.ContentType,
                ContentLength = request.ContentLength,
                QueryString = request.QueryString.ToString(),
                Scheme = request.Scheme,
                Protocol = request.Protocol,
                Method = request.Method,
                Path = request.Path,
                Heads = request.Headers.ToDictionary(d => d.Key, d => d.Value.ToString())
            };
            if (method.Contains("POST") || method.Contains("PUT") || method.Contains("PATCH") || method.Contains("DELETE"))
                requestInfo.Bodys = context.ActionArguments.Select(s => new Body()
                {
                    ModelName = s.Key,
                    Payload = s.Value.ToJson(),
                    Type = s.Value?.GetType().FullName
                });


            var connInfo = new Connection()
            {
                ConnectionId = httpContext.Connection.Id,
                RemoteIpAddress = httpContext.Connection.RemoteIpAddress?.ToString(),
                RemotePort = httpContext.Connection.RemotePort
            };

            var actDescInfo = new ActionDescriptor()
            {
                ActionDescriptorId = actDesc.Id,
                Template = actDesc.AttributeRouteInfo?.Template,
                ControllerName = actDesc.ControllerName,
                ControllerFullName = actDesc.ControllerTypeInfo.FullName,
                ActionName = actDesc.ActionName
            };
            var ctrlSwaggerTagAttr = actDesc.EndpointMetadata.FirstOrDefault(f => f.GetType() == typeof(SwaggerTagAttribute));
            if (ctrlSwaggerTagAttr != null)
                actDescInfo.GroupName = ((SwaggerTagAttribute)ctrlSwaggerTagAttr).Description;
            else
                actDescInfo.GroupName = actDesc.ControllerName;

            var apiRoleDescObject = actDesc.EndpointMetadata.FirstOrDefault(f => f.GetType() == typeof(ApiRoleDescriptionAttribute));
            if (apiRoleDescObject != null)
            {
                var apiRoleDesc = (ApiRoleDescriptionAttribute)apiRoleDescObject;
                actDescInfo.ActionDescription = apiRoleDesc.Name;
            }

            var callLog = new CallLog()
            {
                TraceIdentifier = httpContext.TraceIdentifier,
                IdentityName = httpContext.User.Identity?.Name,
                Request = requestInfo,
                Connection = connInfo,
                ActionDescriptor = actDescInfo
            };

            if (httpContext.User.Identity != null && !httpContext.User.Identity.Name.IsNullOrWhiteSpace())
            {
                var accountId = httpContext.User.FindFirst(ClaimTypes.Sid)?.Value;
                callLog.IdentityName = accountId;
                callLog.SystemAccount = _db.ApteryxSystemAccount.Commands.FindOne(accountId);
            }

            if (!context.ModelState.IsValid)
            {
                requestInfo.ModelState = context.ModelState.IsValid;
                requestInfo.ModelError = context.ModelState.SelectMany(s => s.Value.Errors).Select(s => s.ErrorMessage);

                var options = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
                };
                context.Result = new OkObjectResult(ApteryxResultApi.Fail(ApteryxCodes.字段验证未通过, context.ModelState
                    .Where(w => w.Value.Errors.FirstOrDefault() != null)
                    .Select(s => new FieldValid { Field = s.Key, Error = s.Value.Errors.Select(s1 => s1.ErrorMessage) })));

                var resultValue = ((ObjectResult)context.Result).Value;
                callLog.Response = new Response()
                {
                    StatusCode = context.HttpContext.Response.StatusCode,
                    Result = resultValue.ToJson(),
                    Type = resultValue?.GetType().ToString()
                };
                _db.ApteryxCallLog.Commands.Insert(callLog);
                return;
            }
            _db.ApteryxCallLog.Commands.Insert(callLog);
            return;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity != null && context.HttpContext.User.Identity.IsAuthenticated)
            {
                var method = context.HttpContext.Request.Method;
                var template = $"/{context.ActionDescriptor.AttributeRouteInfo?.Template}";
                var accountId = context.HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value;

                var systemAccount = _db.ApteryxSystemAccount.Commands.FindOne(f => f.Id == accountId);
                if (systemAccount == null)
                {
                    context.Result = new BadRequestObjectResult(ApteryxResultApi.Fail(ApteryxCodes.Unauthorized, $"您的账户可能已被删除，无法继续操作！")) { StatusCode = 200 };
                    return;
                }
                if (systemAccount.IsSuper)
                    return;

                if (!systemAccount.State)
                {
                    context.Result = new BadRequestObjectResult(ApteryxResultApi.Fail(ApteryxCodes.账户已被禁用, $"您的账户已被禁用，无法继续使用！")) { StatusCode = 200 };
                    return;
                }

                var route = _db.ApteryxRoute.Commands.FindOne(f => f.Method == method && f.Path == template);
                if (route != null)
                {
                    var roleRoute = _db.ApteryxRole.Commands.FindOne(f => f.Id == systemAccount.RoleId && f.RouteIds.Contains(route.Id));
                    if (roleRoute != null)
                    {
                        return;
                    }
                    else
                    {
                        var role = _db.ApteryxRole.Commands.FindOne(systemAccount.RoleId);
                        context.Result = new BadRequestObjectResult(ApteryxResultApi.Fail(ApteryxCodes.权限不足, $"角色“{role.Name}”无权限访问“{route.CtrlName}”的“{route.Name}”接口！")) { StatusCode = 200 };
                        return;
                    }
                }
                else
                {
                    context.Result = new BadRequestObjectResult(ApteryxResultApi.Fail(ApteryxCodes.路由不存在, $"路由“{template}”在数据库中未找到！")) { StatusCode = 200 };
                }
            }
            return;
        }
    }
}
