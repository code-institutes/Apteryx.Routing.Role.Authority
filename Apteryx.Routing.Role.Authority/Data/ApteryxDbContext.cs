using Apteryx.MongoDB.Driver.Extend;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Apteryx.Routing.Role.Authority
{
    public class ApteryxDbContext : MongoDbContext
    {
        public ApteryxDbContext(IOptionsMonitor<MongoDBOptions> options) : base(options) { }
        /// <summary>
        /// 系统账户信息
        /// </summary>
        public DbSet<SystemAccount> ApteryxSystemAccount { get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        public DbSet<Role> ApteryxRole { get; set; }
        /// <summary>
        /// 路由
        /// </summary>
        public DbSet<Route> ApteryxRoute {  get; set; }
        /// <summary>
        /// 接口调用日志
        /// </summary>
        public DbSet<CallLog> ApteryxCallLog {  get; set; }
        /// <summary>
        /// 操作日志
        /// </summary>
        public DbSet<OperationLog> ApteryxOperationLog {  get; set; }
    }
}
