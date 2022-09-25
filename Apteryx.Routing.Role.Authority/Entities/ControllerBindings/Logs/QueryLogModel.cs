namespace Apteryx.Routing.Role.Authority
{
    public class QueryLogModel : BaseQueryModel
    {
        /// <summary>
        /// 行为
        /// </summary>
        public ActionMethods? Method { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        public string? Key { get; set; }

        /// <summary>
        /// 系统账户ID
        /// </summary>
        public string? AccountId { get; set; }

        /// <summary>
        /// 日志组ID
        /// </summary>
        public string? GroupId { get; set; }
    }
}
