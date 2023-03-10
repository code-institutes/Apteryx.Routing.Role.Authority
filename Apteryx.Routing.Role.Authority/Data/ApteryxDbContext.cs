using Apteryx.MongoDB.Driver.Extend;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Apteryx.Routing.Role.Authority
{
    public class ApteryxDbContext : MongoDbProvider
    {
        public ApteryxDbContext(IOptionsMonitor<MongoDBOptions> options) : base(options) { }
        /// <summary>
        /// 系统账户信息
        /// </summary>
        public IMongoCollection<SystemAccount> SystemAccounts => Database.GetCollection<SystemAccount>("ApteryxSystemAccount");
        /// <summary>
        /// 角色
        /// </summary>
        public IMongoCollection<Role> Roles => Database.GetCollection<Role>("ApteryxRole");
        /// <summary>
        /// 路由
        /// </summary>
        public IMongoCollection<Route> Routes => Database.GetCollection<Route>("ApteryxRoute");
        /// <summary>
        /// 接口调用日志
        /// </summary>
        public IMongoCollection<CallLog> CallLogs => Database.GetCollection<CallLog>("ApteryxCallLog");
        /// <summary>
        /// 操作日志
        /// </summary>
        public IMongoCollection<OperationLog> OperationLogs => Database.GetCollection<OperationLog>("ApteryxOperationLog");
    }
}
