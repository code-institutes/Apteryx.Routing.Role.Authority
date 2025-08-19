using apteryx.common.extend.Helpers;
using Apteryx.MongoDB.Driver.Extend;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using MongoDB.Bson;
using MongoDB.Driver;
using Swashbuckle.AspNetCore.Annotations;
using System.Reflection;

namespace Apteryx.Routing.Role.Authority.Controllers;

[Authorize(AuthenticationSchemes = "apteryx")]
[SwaggerTag("路由服务")]
[Route("apteryx/route")]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "apteryx1.0")]
[SwaggerResponse((int)ApteryxCodes.Unauthorized, null, typeof(ApteryxResult))]
[SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult))]
public class RouteController : ControllerBase
{
    private readonly ApteryxDbContext _db;
    private readonly ApteryxInitializeDataService _initDataService;
    private readonly IActionDescriptorCollectionProvider actionDescriptor;
    private readonly ApteryxOperationLogService _log;

    public RouteController(IActionDescriptorCollectionProvider collectionProvider, ApteryxDbContext mongoDbContext,
        ApteryxInitializeDataService initializeDataService, ApteryxOperationLogService logService)
    {
        this._db = mongoDbContext;
        this.actionDescriptor = collectionProvider;
        this._initDataService = initializeDataService;
        this._log = logService;
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "添加路由",
        OperationId = "Post",
        Tags = new[] { "Route" }
    )]
    [ApiRoleDescription("A", "添加")]
    [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult<Route>))]
    public async Task<IActionResult> Post([FromBody] AddRouteModel model)
    {
        var path = model.Path.Trim();
        var method = model.Method.Trim();
        var action = await _db.ApteryxRoute.FindOneAsync(f => f.Path == path && f.Method == method);
        if (action != null)
            return Ok(ApteryxResultApi.Fail(ApteryxCodes.路由已存在));

        var route = new Route()
        {
            ServiceName = Assembly.GetEntryAssembly()?.GetName().Name,
            Id = ObjectId.GenerateNewId().ToString(),
            Name = model.Name.Trim(),
            CtrlName = model.CtrlName.Trim(),
            CtrlFullName = model.CtrlName.Trim(),
            Description = model.Description.Trim(),
            Method = method,
            Tag = model.Tag.Trim(),
            Path = path
        };
        await _db.ApteryxRoute.AddAsync(route);
        //记录日志
        await _log.CreateAsync(route, null);
        return Ok(ApteryxResultApi.Susuccessful(route));
    }

    [HttpPut]
    [SwaggerOperation(
        Summary = "编辑路由",
        OperationId = "Put",
        Tags = new[] { "Route" }
    )]
    [ApiRoleDescription("B", "编辑")]
    [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult<Route>))]
    [SwaggerResponse((int)ApteryxCodes.路由无权修改, null, typeof(ApteryxResult))]
    public async Task<IActionResult> Put([FromBody] EditRouteModel model)
    {
        var routeId = model.Id;
        var path = model.Path.Trim();
        var method = model.Method.Trim();

        var route = await _db.ApteryxRoute.FindOneAsync(f => f.Id == routeId);
        if (route == null)
            return Ok(ApteryxResultApi.Fail(ApteryxCodes.路由不存在));

        if (route.AddType != AddTypes.人工)
            return Ok(ApteryxResultApi.Fail(ApteryxCodes.路由无权修改, "只能编辑手动添加的路由"));

        var check = await _db.ApteryxRoute.FindOneAsync(f => f.Path == path && f.Method == method);
        if (check != null)
            if (check.Id != routeId)
                return Ok(ApteryxResultApi.Fail(ApteryxCodes.路由已存在, "已存在相同路由数据"));

        route.CtrlName = model.CtrlName.Trim();
        route.Description = model.Description.Trim();
        route.Method = method;
        route.Path = path;

        var result = await _db.ApteryxRoute.FindOneAndReplaceOneAsync(f => f.Id == route.Id, route);
        //记录日志
        await _log.CreateAsync(route, result);

        return Ok(ApteryxResultApi.Susuccessful(route));
    }

    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "获取指定路由",
        OperationId = "Get",
        Tags = new[] { "Route" }
    )]
    [ApiRoleDescription("C", "获取")]
    [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult<Route>))]
    public async Task<IActionResult> Get([SwaggerParameter("路由ID", Required = true)] string id)
    {
        return Ok(ApteryxResultApi.Susuccessful(await _db.ApteryxRoute.FindOneAsync(f => f.Id == id)));
    }

    [HttpDelete]
    [Route("{id}")]
    [SwaggerOperation(
        Summary = "删除路由",
        OperationId = "Delete",
        Tags = new[] { "Route" }
    )]
    [ApiRoleDescription("D", "删除")]
    [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult))]
    public async Task<IActionResult> Delete([SwaggerParameter("路由ID", Required = true)] string id)
    {
        var sysAccountId = HttpContext.GetAccountId();
        var route = await _db.ApteryxRoute.FindOneAsync(f => f.Id == id);
        if (route == null)
            return Ok(ApteryxResultApi.Fail(ApteryxCodes.路由不存在));
        if (route.AddType != AddTypes.人工)
            return Ok(ApteryxResultApi.Fail(ApteryxCodes.路由无权删除, "只能删除手动添加的路由"));

        //将路由从所有角色中解除关联
        await _db.ApteryxRole.UpdateManyAsync(u => u.RouteIds.Contains(id), Builders<Role>.Update.Pull(p => p.RouteIds, id));
        //删除路由
        var result = await _db.ApteryxRoute.FindOneAndDeleteAsync(d => d.Id == id);
        //记录日志
        await _log.CreateAsync(null, result);

        return Ok(ApteryxResultApi.Susuccessful());
    }
    
    [HttpPost("query")]
    [SwaggerOperation(
        Summary = "查询",
        OperationId = "Query",
        Tags = new[] { "Route" }
    )]
    [ApiRoleDescription("E", "查询")]
    [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult<PageList<Route>>))]
    public async Task<IActionResult> PostQuery([FromBody] QueryRouteModel model)
    {
        var title = model.CtrlName;
        var method = model.Method;
        var path = model.Path;

        var query = _db.ApteryxRoute.AsMongoCollection.AsQueryable().AsQueryable();
        if (!model.IsShowMustHave)
            query = query.Where(w => w.IsMustHave == false);

        if (title!= null && !title.IsNullOrWhiteSpace())
            query = query.Where(w => w.CtrlName.Contains(title));

        if (method != null && !method.IsNullOrWhiteSpace())
            query = query.Where(w => w.Method.Contains(method));

        if (path != null && !path.IsNullOrWhiteSpace())
            query = query.Where(w => w.Path.Contains(path));

        var data = await query.OrderByDescending(o => o.Id).ToPageListAsync(model.Page, model.Limit);

        return Ok(ApteryxResultApi.Susuccessful(data));
    }

    [HttpGet("all")]
    [SwaggerOperation(
                   Summary = "获取所有路由",
                   OperationId = "GetAll",
                   Tags = new[] { "Route" }
                          )]
    [ApiRoleDescription("F", "获取所有路由")]
    [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult<IEnumerable<Route>>))]
    public IActionResult GetAll()
    {
        return Ok(ApteryxResultApi.Susuccessful(_db.ApteryxRoute.FindAll()));
    }


#if DEBUG
    [AllowAnonymous]
#endif
    [HttpGet("refresh")]
    [SwaggerOperation(
        Summary = "刷新",
        OperationId = "Refresh",
        Tags = new[] { "Route" }
    )]
    [ApiRoleDescription("G", "刷新")]
    [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult<IEnumerable<ResultGroupRouteModel>>))]
    public  ActionResult<ApteryxResult<List<Route>>> GetRefresh()
    {
        _initDataService.RefreshRoute();

        var item = _db.ApteryxRoute.FindAll().GroupBy(g => g.CtrlName).Select(s => new ResultGroupRouteModel()
        {
            CtrlName = s.Key,
            Routes = s.Select(ss => ss)
        });

        return Ok(ApteryxResultApi.Susuccessful(item));
    }
}
