﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Apteryx.Routing.Role.Authority
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class TokenBuilder
    {
        private SecurityKey? securityKey;
        private string subject = "";
        private string issuer = "";
        private string audience = "";
        private Dictionary<string, string> claims = new Dictionary<string, string>();
        private int expiryInMinutes = 5;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="securityKey"></param>
        /// <returns></returns>
        private TokenBuilder AddSecurityKey(SecurityKey securityKey)
        {
            this.securityKey = securityKey;
            return this;
        }

        public TokenBuilder AddSecurityKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            return AddSecurityKey(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        public TokenBuilder AddSubject(string subject)
        {
            this.subject = subject;
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="issuer"></param>
        /// <returns></returns>
        public TokenBuilder AddIssuer(string issuer)
        {
            this.issuer = issuer;
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="audience"></param>
        /// <returns></returns>
        public TokenBuilder AddAudience(string audience)
        {
            this.audience = audience;
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public TokenBuilder AddClaim(string type, string value)
        {
            this.claims.Add(type, value);
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        public TokenBuilder AddClaims(Dictionary<string, string> claims)
        {
            this.claims.Union(claims);
            return this;
        }
        /// <summary>
        /// 添加过期时间（秒）
        /// </summary>
        /// <param name="expiryInMinutes"></param>
        /// <returns></returns>
        public TokenBuilder AddExpiry(int expiryInMinutes)
        {
            this.expiryInMinutes = expiryInMinutes;
            return this;
        }
        /// <summary>
        /// 生成JwtToken
        /// </summary>
        /// <returns></returns>
        public JwtSecurityToken Build()
        {
            EnsureArguments();
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, this.subject),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }.Union(this.claims.Select(item => new Claim(item.Key, item.Value)));

            return new JwtSecurityToken(
                              issuer: this.issuer,
                              audience: this.audience,
                              claims: claims,
                              notBefore: DateTime.UtcNow,
                              expires: DateTime.UtcNow.AddSeconds(expiryInMinutes),
                              signingCredentials: new SigningCredentials(this.securityKey,SecurityAlgorithms.HmacSha256));
        }

        private void EnsureArguments()
        {
            if (this.securityKey == null)
                throw new ArgumentNullException("Security Key");

            if (string.IsNullOrEmpty(this.subject))
                throw new ArgumentNullException("Subject");

            if (string.IsNullOrEmpty(this.issuer))
                throw new ArgumentNullException("Issuer");

            if (string.IsNullOrEmpty(this.audience))
                throw new ArgumentNullException("Audience");
        }
    }
}
