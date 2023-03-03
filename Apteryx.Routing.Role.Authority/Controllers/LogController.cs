using apteryx.common.extend.Helpers;
using Apteryx.MongoDB.Driver.Extend;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Swashbuckle.AspNetCore.Annotations;

namespace Apteryx.Routing.Role.Authority.Controllers
{
#if !DEBUG
    [Authorize(AuthenticationSchemes = "apteryx")]
#endif
    [SwaggerTag("日志服务")]
    [Route("cgi-bin/apteryx/log")]
    [Produces("application/json")]
    [ApiExplorerSettings(GroupName = "zh1.0")]
    [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult))]
    public class LogController : ControllerBase
    {
        private readonly ApteryxDbContext _db;

        public LogController(IConfiguration config, ApteryxDbContext mongoDbContext)
        {
            _db = mongoDbContext;
        }

        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "获取指定日志信息",
            OperationId = "Get",
            Tags = new[] { "Log" }
        )]
        [ApiRoleDescription("A", "获取")]
        [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult<Log>))]
        public async Task<IActionResult> Get([SwaggerParameter("日志ID", Required = true)] string id)
        {
            return Ok(ApteryxResultApi.Susuccessful(await _db.Logs.FindOneAsync(f => f.Id == id)));
        }

        [HttpPost("query")]
        [SwaggerOperation(
            Summary = "查询",
            OperationId = "Query",
            Tags = new[] { "Log" }
        )]
        [ApiRoleDescription("B", "查询")]
        [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult<PageList<LogExtModel>>))]
        public async Task<IActionResult> PostQuery([FromBody] QueryLogModel model)
        {
            var page = model.Page;
            var limit = model.Limit;
            var method = model.Method;
            var accountId = model.AccountId;
            var groupId = model.GroupId;
            var key = model.Key;

            var query = _db.Logs.AsQueryable().AsQueryable();
            if (method != null)
                query = query.Where(w => w.ActionMethod == method);

            if (!accountId.IsNullOrWhiteSpace())
                query = query.Where(w => w.SystemAccountId == accountId);

            if (!groupId.IsNullOrWhiteSpace())
                query = query.Where(w => w.GroupId == groupId);

            if (!key.IsNullOrWhiteSpace())
                query = query.Where(w => w.ActionName.Contains(key) || w.Source.Contains(key) || w.After.Contains(key));

            //var count = query.Count();
            //var data = query.OrderByDescending(o => o.Id).ToPageData(page, limit);

            //var listLog = data.Select(s =>
            //{
            //    var sysAccount = _db.SystemAccounts.FindOne(f => f.Id == s.SystemAccountId);
            //    var role = _db.Roles.FindOne(f => f.Id == sysAccount.RoleId);
            //    return new LogExtModel()
            //    {
            //        Id = s.Id,
            //        AccountInfo = new ResultSystemAccountRoleModel(sysAccount, role),
            //        ActionMethod = s.ActionMethod,
            //        ActionName = s.ActionName,
            //        TableName = s.MongoCollectionName,
            //        CreateTime = s.CreateTime
            //    };
            //});
            //return Ok(ApteryxResultApi.Susuccessful(new PageList<LogExtModel>(count, listLog)));

            var data = query.OrderByDescending(o => o.Id).ToPageList(s =>
            {
                var sysAccount = _db.SystemAccounts.FindOne(f => f.Id == s.SystemAccountId);
                var role = _db.Roles.FindOne(f => f.Id == sysAccount.RoleId);
                return new LogExtModel()
                {
                    Id = s.Id,
                    AccountInfo = new ResultSystemAccountRoleModel(sysAccount, role),
                    ActionMethod = s.ActionMethod,
                    ActionName = s.ActionName,
                    TableName = s.MongoCollectionName,
                    CreateTime = s.CreateTime
                };
            }, page, limit);
            return Ok(ApteryxResultApi.Susuccessful(data));
        }
    }
}