using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Apteryx.Routing.Role.Authority.Controllers;

#if !DEBUG
[Authorize(AuthenticationSchemes = "apteryx")]
#endif
[SwaggerTag("日志服务")]
[Route("apteryx/log")]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "apteryx1.0")]
[SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult))]
public class LogController : ControllerBase
{
    private readonly ApteryxOperationLogService _log;

    public LogController(ApteryxOperationLogService logService)
    {
        this._log = logService;
    }

    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "获取指定日志信息",
        OperationId = "Get",
        Tags = new[] { "Log" }
    )]
    [ApiRoleDescription("A", "获取")]
    [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult<OperationLog>))]
    public async Task<IApteryxResult> Get([SwaggerParameter("日志ID", Required = true)] string id)
    {
        return await _log.GetAsync(id);
    }

    [HttpPost("query")]
    [SwaggerOperation(
        Summary = "查询",
        OperationId = "Query",
        Tags = new[] { "Log" }
    )]
    [ApiRoleDescription("B", "查询")]
    [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult<PageList<OperationLog>>))]
    public async Task<IApteryxResult> PostQuery([FromBody] QueryOperationLogModel model)
    {
        return await _log.Query(model);
    }

    [HttpGet("all/{dataId}")]
    [SwaggerOperation(
        Summary = "获取全部",
        OperationId = "GetAll",
        Tags = new[] { "Log" }
    )]
    [ApiRoleDescription("C", "获取全部")]
    [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult<PageList<OperationLog>>))]
    public async Task<IApteryxResult> GetAll([SwaggerParameter("数据ID（Mongo文档ID）", Required = true)] string dataId)
    {
        var result = await _log.GetAllAsync(dataId);
        return ApteryxResultApi.Susuccessful(result.OrderBy(o=>o.CreateTime).ToList());
    }
}