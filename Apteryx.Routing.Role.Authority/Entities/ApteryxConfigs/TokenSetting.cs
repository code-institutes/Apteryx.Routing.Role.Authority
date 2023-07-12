namespace Apteryx.Routing.Role.Authority
{
    public sealed class TokenSetting
    {
        /// <summary>
        /// KEY
        /// </summary>
        public required string Key { get; set; }

        /// <summary>
        /// 发行人
        /// </summary>
        public required string Issuer { get; set; }

        /// <summary>
        /// 订阅人
        /// </summary>
        public required string Audience { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public required int Expires { get; set; }
    }
}
