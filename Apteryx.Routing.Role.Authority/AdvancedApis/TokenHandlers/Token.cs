using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Apteryx.Routing.Role.Authority
{
    public sealed class Token<T>
    {
        public Token(JwtSecurityToken securityToken) : this(securityToken, default(T)) { }
        public Token(JwtSecurityToken securityToken, string aesKey, string aesIv) : this(securityToken, aesKey, aesIv, default(T)) { }
        public Token(JwtSecurityToken securityToken, T? obj)
        {
            this.AccessToken = new JwtSecurityTokenHandler().WriteToken(securityToken);
            this.ValidTo = securityToken.ValidTo;
            this.AppendInfo = obj;
        }
        public Token(JwtSecurityToken accessSecurityToken, string aesKey, string aesIv, T? obj)
        {
            this.AccessToken = AES256HandlerApi.Encrypt(new JwtSecurityTokenHandler().WriteToken(accessSecurityToken), aesKey, aesIv);
            this.ValidTo = accessSecurityToken.ValidTo;
            this.AppendInfo = obj;
        }
        /// <summary>
        /// 账户信息
        /// </summary>
        public T? AppendInfo { get; set; }

        /// <summary>
        /// 访问凭证(JWT协议)
        /// </summary>
        public string AccessToken { get; set; }
        /// <summary>
        /// 前缀
        /// </summary>
        public string TokenType => JwtBearerDefaults.AuthenticationScheme;
        /// <summary>
        /// 有效截止时间
        /// </summary>
        public DateTime ValidTo { get; set; }
    }
}
