namespace Apteryx.Routing.Role.Authority
{
    public sealed class LogExtModel
    {
        /// <summary>
        /// 日志ID
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        public string? TableName { get; set; }

        /// <summary>
        /// 行为
        /// </summary>
        public ActionMethods ActionMethod { get; set; }

        /// <summary>
        /// 方法/动作名
        /// </summary>
        public string? ActionName { get; set; }

        /// <summary>
        /// 记录时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 管理员信息
        /// </summary>
        public ResultSystemAccountRoleModel? AccountInfo { get; set; }
    }
}
