//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Configuration;
//using Swashbuckle.AspNetCore.Annotations;

//namespace Apteryx.Routing.Role.Authority.Controllers
//{
//#if !DEBUG
//    [Authorize(AuthenticationSchemes = "apteryx")]
//#endif
//    [SwaggerTag("调用日志服务")]
//    [Route("cgi-bin/apteryx/call/log")]
//    [Produces("application/json")]
//    [ApiExplorerSettings(GroupName = "zh1.0")]
//    [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult))]
//    public class CallLogController : ControllerBase
//    {
//        private readonly ApteryxDbContext _db;

//        public CallLogController(IConfiguration config, ApteryxDbContext mongoDbContext)
//        {
//            _db = mongoDbContext;
//        }
//    }
//}
