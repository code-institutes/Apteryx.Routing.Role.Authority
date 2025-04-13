using Apteryx.MongoDB.Driver.Extend;
namespace Apteryx.Routing.Role.Authority
{
    public sealed class ApteryxConfig
    {
        /// <summary>
        /// 认知方案(缺省值：apteryx)
        /// </summary>
        public string AuthenticationScheme { get; set; } = "apteryx";
        /// <summary>
        /// Token配置
        /// </summary>
        public required TokenSetting TokenConfig { get; set; }

        /// <summary>
        /// 是否使用加密Token
        /// </summary>
        public bool UseSecurityToken { get; set; } = false;

        /// <summary>
        /// AES加密配置
        /// </summary>
        public AES256Setting? AESConfig { get; set; }

        /// <summary>
        /// MongoDB连接配置
        /// </summary>
        public required MongoDBOptions MongoDBOptions { get; set; }
    }
}
