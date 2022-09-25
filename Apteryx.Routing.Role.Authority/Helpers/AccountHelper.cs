using Microsoft.AspNetCore.Http;

namespace Apteryx.Routing.Role.Authority
{
    public static class AccountHelper
    {
        public static string GetAccountId(this HttpContext context)
        {
            return context.User.Identity.Name;
        }
    }
}
