using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Apteryx.Routing.Role.Authority
{
    public static class AccountHelper
    {
        /// <summary>
        /// 获取当前登录账户ID
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string GetAccountId(this HttpContext context)
        {
            if (context.User.Identity == null || context.User.Identity.Name == null)
                throw new Exception("账户身份验证失败！");

            return context.User.FindFirst(ClaimTypes.Sid)?.Value;
        }
        /// <summary>
        /// 获取当前登录账户名称
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string GetAccountName(this HttpContext context)
        {
            if (context.User.Identity == null || context.User.Identity.Name == null)
                throw new Exception("账户身份验证失败！");

            return context.User.Identity.Name;
        }
    }
}
