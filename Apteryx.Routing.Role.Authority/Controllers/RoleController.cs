using Apteryx.MongoDB.Driver.Extend;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Swashbuckle.AspNetCore.Annotations;

namespace Apteryx.Routing.Role.Authority.Controllers;

[Authorize(AuthenticationSchemes = "apteryx")]
[SwaggerTag("角色服务")]
[Route("apteryx/role")]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "apteryx1.0")]
[SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult))]
public class RoleController : ControllerBase
{
    private readonly ApteryxDbContext _db;
    private readonly ApteryxOperationLogService _log;

    public RoleController(ApteryxDbContext mongoDbContext, ApteryxOperationLogService logService)
    {
        this._db = mongoDbContext;
        this._log = logService;
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "添加角色",
        OperationId = "Post",
        Tags = new[] { "Role" }
    )]
    [ApiRoleDescription("A", "添加")]
    [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult<Role>))]
    public async Task<IActionResult> Post([FromBody] AddRoleModel model)
    {
        var role = await _db.ApteryxRole.Commands.FindOneAsync(f => f.Name == model.Name.Trim());
        if (role != null)
            return Ok(ApteryxResultApi.Fail(ApteryxCodes.角色已存在, $"角色名：\"{model.Name}\"已存在"));

        role = new Role()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Name = model.Name.Trim(),
            Description = model.Description?.Trim(),
            RouteIds = _db.ApteryxRoute.Where(w => w.IsMustHave == true).Select(s => s.Id).ToList()
        };

        model.RouteIds.Where(w => _db.ApteryxRoute.Commands.FindOne(a => a.Id == w && a.IsMustHave == false) != null).ToList().ForEach(f => role.RouteIds.Add(f));
        await _db.ApteryxRole.Commands.InsertAsync(role);
        //记录日志
        //await _log.CreateAsync(role, null);
        await _log.CreateAsync(role, null);

        return Ok(ApteryxResultApi.Susuccessful(role));
    }

    [HttpPut]
    [SwaggerOperation(
        Summary = "编辑角色",
        OperationId = "Put",
        Tags = new[] { "Role" }
    )]
    [ApiRoleDescription("B", "编辑")]
    [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult<Role>))]

    public async Task<IActionResult> Put([FromBody] EditRoleModel model)
    {
        var roleId = model.Id;
        var role = await _db.ApteryxRole.Commands.FindOneAsync(f => f.Id == roleId);
        if (role == null)
            return Ok(ApteryxResultApi.Fail(ApteryxCodes.角色不存在, $"角色不存在,ID:{roleId}"));

        if (role.AddType == AddTypes.程序 && role.Name == "管理员")
            return Ok(ApteryxResultApi.Fail(ApteryxCodes.系统角色, "禁止操作系统默认角色！"));

        if (_db.ApteryxRole.Commands.FindOne(a => a.Name == model.Name && a.Id != model.Id) != null)
            return Ok(ApteryxResultApi.Fail(ApteryxCodes.角色已存在, $"角色名：\"{model.Name}\"已存在"));

        role.Name = model.Name;
        role.Description = model.Description;
        role.RouteIds = _db.ApteryxRoute.Where(w => w.IsMustHave == true).Select(s => s.Id).ToList();

        model.RouteIds.Where(w => _db.ApteryxRoute.Commands.FindOne(a => a.Id == w && a.IsMustHave == false) != null).ToList().ForEach(f => role.RouteIds.Add(f));
        var result = await _db.ApteryxRole.Commands.FindOneAndReplaceOneAsync(r => r.Id == role.Id, role);
        //记录日志
        await _log.CreateAsync(role, result);

        return Ok(ApteryxResultApi.Susuccessful(role));
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "获取账户所有权限",
        OperationId = "Own",
        Description = "获取当前登录账户的权限(包含所有接口)",
        Tags = new[] { "Role" }
    )]
    [ApiRoleDescription("C", "获取拥有/非拥有的权限", isMustHave: true)]
    [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult<ResultOwnRouteModel>))]
    public async Task<IActionResult> GetOwn()
    {
        var accountId = HttpContext.GetAccountId();
        var account = await _db.ApteryxSystemAccount.Commands.FindOneAsync(f => f.Id == accountId);
        var routes = _db.ApteryxRoute.Commands.FindAll();

        var data = new ResultOwnRouteModel()
        {
            Role = await _db.ApteryxRole.Commands.FindOneAsync(account.RoleId),
            GroupRoutes = routes.GroupBy(g => g.CtrlName).Select(s => new GroupRouteModel()
            {
                Title = s.Key,
                RouteInfo = s.Select(ss => new RouteInfoModel()
                {
                    IsOwn = _db.ApteryxRole.Commands.FindOne(f => f.Id == account.RoleId && f.RouteIds.Contains(ss.Id)) != null,
                    Route = ss
                })
            })
        };
        return Ok(ApteryxResultApi.Susuccessful(data));
    }

    [HttpGet("own")]
    [SwaggerOperation(
        Summary = "获取账户所有权限",
        OperationId = "OnlyOwn",
        Description = "获取当前登录账户的权限",
        Tags = new[] { "Role" }
    )]
    [ApiRoleDescription("C1", "获取拥有权限", isMustHave: true)]
    [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult<IEnumerable<Route>>))]
    public async Task<IActionResult> GetOnlyOwn()
    {
        var accountId = HttpContext.GetAccountId();
        var account = await _db.ApteryxSystemAccount.Commands.FindOneAsync(f => f.Id == accountId);
        var routes = _db.ApteryxRoute.Commands.FindAll();
        var role = await _db.ApteryxRole.Commands.FindOneAsync(account.RoleId);
        var data = role.RouteIds.Select(s => routes.FirstOrDefault(f => f.Id == s));
        return Ok(ApteryxResultApi.Susuccessful(data));
    }

    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "获取指定角色的权限",
        OperationId = "Get",
        Tags = new[] { "Role" }
    )]
    [ApiRoleDescription("D", "获取角色权限")]
    [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult<IEnumerable<ResultOwnRouteModel>>))]
    public async Task<IActionResult> Get([SwaggerParameter("角色ID", Required = true)] string id)
    {
        var role = await _db.ApteryxRole.Commands.FindOneAsync(f => f.Id == id);
        if (role == null)
            return Ok(ApteryxResultApi.Fail(ApteryxCodes.角色不存在, $"角色不存在,ID:{id}"));

        var routes = _db.ApteryxRoute.Commands.FindAll();

        var data = new ResultOwnRouteModel()
        {
            Role = await _db.ApteryxRole.Commands.FindOneAsync(id),
            GroupRoutes = routes.GroupBy(g => g.CtrlName).Select(s => new GroupRouteModel()
            {
                Title = s.Key,
                RouteInfo = s.Select(ss => new RouteInfoModel()
                {
                    IsOwn = _db.ApteryxRole.Commands.FindOne(f => f.Id == id && f.RouteIds.Contains(ss.Id)) != null,
                    Route = ss
                })
            })
        };
        return Ok(ApteryxResultApi.Susuccessful(data));
    }

    [HttpDelete]
    [Route("{id}")]
    [SwaggerOperation(
        Summary = "删除角色",
        OperationId = "Delete",
        Tags = new[] { "Role" }
    )]
    [ApiRoleDescription("E", "删除")]
    [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult))]
    public async Task<IActionResult> Delete([SwaggerParameter("角色ID", Required = true)] string id)
    {
        var role = await _db.ApteryxRole.Commands.FindOneAsync(d => d.Id == id);
        if (role == null)
            return Ok(ApteryxResultApi.Fail(ApteryxCodes.角色不存在, $"角色不存在,ID:{id}"));

        if (role.AddType == AddTypes.程序)
            return Ok(ApteryxResultApi.Fail(ApteryxCodes.系统角色, "系统默认角色禁止删除！"));

        await _db.ApteryxRole.Commands.DeleteOneAsync(d => d.Id == id);

        var groupId = ObjectId.GenerateNewId().ToString();
        //记录日志
        await _log.CreateAsync(null, role);

        foreach (var sysAccount in _db.ApteryxSystemAccount.Commands.Find(w => w.RoleId == id).ToEnumerable())
        {
            var sysAccountResult = _db.ApteryxSystemAccount.Commands.DeleteOne(sysAccount.Id);
            if (sysAccountResult.IsAcknowledged)
            {
                //记录日志
                await _log.CreateAsync<BaseMongoEntity>(null, null, "删除角色联动删除账户");
            }
        }
        return Ok(ApteryxResultApi.Susuccessful());
    }

    [HttpPost("query")]
    [SwaggerOperation(
        Summary = "查询",
        OperationId = "Query",
        Tags = new[] { "Role" }
    )]
    [ApiRoleDescription("F", "查询")]
    [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult<PageList<Role>>))]
    public async Task<IActionResult> PostQuery([FromBody] QueryRoleModel model)
    {
        var page = model.Page;
        var limit = model.Limit;
        var key = model.Key;

        var query = _db.ApteryxRole.Native.AsQueryable();

        if (!string.IsNullOrEmpty(key))
            query = query.Where(w => w.Name.Contains(key) || w.Description.Contains(key));

        var data = await query.OrderByDescending(d => d.Id).ToPageListAsync(page, limit);
        return Ok(ApteryxResultApi.Susuccessful(data));
    }

    [HttpGet("all")]
    [SwaggerOperation(
        Summary = "获取所有角色",
        OperationId = "All",
        Tags = new[] { "Role" })]
    [ApiRoleDescription("G", "获取所有")]
    [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult<IEnumerable<Role>>))]
    public IActionResult GetAll()
    {
        return Ok(ApteryxResultApi.Susuccessful(_db.ApteryxRole.Commands.FindAll()));
    }

    //[HttpGet("report/usage/{roleId}")]
    //[SwaggerOperation(
    //    Summary = "使用率",
    //    Description = "统计指定角色所有账户的使用率",
    //    OperationId = "Item",
    //    Tags = new[] { "Role" }
    //)]
    //[ApiRoleDescription("G", "统计使用率")]
    //[SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult<ReportUsageRoleModel>))]
    //public async Task<IActionResult> Usage(string roleId)
    //{
    //    var role = await _db.Roles.Commands.FindOneAsync(f => f.Id == roleId);
    //    if (role == null)
    //        return Ok(ApteryxResultApi.Fail(ApteryxCodes.角色不存在));

    //    var accounts = _db.SystemAccounts.Commands.Where(w => w.RoleId == role.Id);

    //    var item = accounts.Select(s =>
    //    {
    //        var usage = new ReportUsageRoleModel()
    //        {
    //            Account = s,
    //            LogNum = _db.Logs.CountDocuments(log => log.SystemAccountId == s.Id)
    //        };
    //        usage.Account.Password = "";
    //        return usage;
    //    }).OrderByDescending(d => d.LogNum);
    //    return Ok(ApteryxResultApi.Susuccessful(item.ToList()));
    //}
}
