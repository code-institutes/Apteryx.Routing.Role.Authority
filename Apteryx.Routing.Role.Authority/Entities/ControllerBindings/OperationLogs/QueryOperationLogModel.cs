namespace Apteryx.Routing.Role.Authority
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class QueryOperationLogModel : BaseQueryModel
    {
        /// <summary>
        /// 关键词
        /// </summary>
        public string? KeyWord { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? Remarks { get; set; }
        /// <summary>
        /// 系统账户ID
        /// </summary>
        public string? AccountId { get; set; }
        /// <summary>
        /// 日志组ID
        /// </summary>
        public string? GroupId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? GroupName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? ActionName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? ActionDescription { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? ActionMethod { get; set; }
    }
}
