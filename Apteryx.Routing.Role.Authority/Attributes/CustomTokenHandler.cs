using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Apteryx.Routing.Role.Authority.Attributes
{
    public class CustomTokenHandler : JwtSecurityTokenHandler
    {
        private readonly string key;
        private readonly string iv;

        public CustomTokenHandler(string key, string iv)
        {
            if (key.Length != 32)
                throw new ArgumentException("AES Key length must be 32 bytes for AES-256.");
            if (iv.Length != 16)
                throw new ArgumentException("AES IV length must be 16 bytes.");

            this.key = key;
            this.iv = iv;
        }

        public override bool CanReadToken(string securityToken)
        {
            if (string.IsNullOrWhiteSpace(securityToken))
                return false;
            return true;
        }

        public override ClaimsPrincipal ValidateToken(string token, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            string decryptedToken = "";
            try
            {
                decryptedToken = AES256HandlerApi.Decrypt(token, key, iv);
            }
            catch
            {
                decryptedToken = token;
            }
            return base.ValidateToken(decryptedToken, validationParameters, out validatedToken);
        }
    }
}
