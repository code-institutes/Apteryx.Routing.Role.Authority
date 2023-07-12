using Microsoft.AspNetCore.Http;

namespace Apteryx.Routing.Role.Authority
{
    public static class AccountHelper
    {
        public static string GetAccountId(this HttpContext context)
        {
            if (context.User.Identity == null || context.User.Identity.Name == null)
                throw new Exception("账户身份验证失败！");

            return context.User.Identity.Name;
        }
    }
}
