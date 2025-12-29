namespace Apteryx.Routing.Role.Authority
{
    /// <summary>
    /// 查询操作日志模型
    /// </summary>
    public sealed class QueryOperationLogModel : BaseQueryModel
    {
        /// <summary>
        /// 关键词
        /// </summary>
        public string? KeyWord { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string? Remarks { get; set; }
        /// <summary>
        /// 系统账户ID
        /// </summary>
        public string? AccountId { get; set; }
        /// <summary>
        /// 组ID
        /// </summary>
        public string? GroupId { get; set; }
        /// <summary>
        /// 组名
        /// </summary>
        public string? GroupName { get; set; }
        /// <summary>
        /// 操作名
        /// </summary>
        public string? ActionName { get; set; }
        /// <summary>
        /// 操作描述
        /// </summary>
        public string? ActionDescription { get; set; }
        /// <summary>
        /// 操作方法
        /// </summary>
        public string? ActionMethod { get; set; }
        /// <summary>
        /// 数据ID（Mongo文档ID）
        /// </summary>
        public string? DataId {  get; set; }
    }
}
