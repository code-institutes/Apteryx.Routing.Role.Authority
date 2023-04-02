namespace Apteryx.Routing.Role.Authority
{
    /// <summary>
    /// 查询系统账户模型
    /// </summary>
    public sealed class QuerySystemAccountModel : BaseQueryModel
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// 电子邮件
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public string? RoleId { get; set; }
    }
}
