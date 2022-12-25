using apteryx.common.extend.Helpers;
using Apteryx.MongoDB.Driver.Extend;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using System.Text;

namespace Apteryx.Routing.Role.Authority.Controllers
{
    [Authorize(AuthenticationSchemes = "apteryx")]
    [SwaggerTag("系统账户服务")]
    [Route("cgi-bin/apteryx/system/account")]
    [Produces("application/json")]
    [ApiExplorerSettings(GroupName = "zh1.0")]
    [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult))]
    [SwaggerResponse((int)ApteryxCodes.字段验证未通过, null, typeof(ApteryxResult<IEnumerable<FieldValid>>))]
    public class SystemAccountController : Controller
    {
        private readonly ApteryxDbContext _db;

        public readonly ApteryxConfig _jwtConfig;
        public SystemAccountController(ApteryxConfig jwtConfig, ApteryxDbContext mongoDbContext)
        {
            _db = mongoDbContext;
            _jwtConfig = jwtConfig;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("log-in")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "账户登陆",
            Description = "提交账号和密码换取访问令牌",
            OperationId = "LogIn",
            Tags = new[] { "SystemAccount" }
        )]
        [SwaggerResponse(200, null, typeof(ApteryxResult<Jwt<ResultSystemAccountRoleModel>>))]
        [SwaggerResponse((int)ApteryxCodes.账号或密码错误, null, typeof(ApteryxResult))]
        public async Task<IActionResult> LogIn([FromBody] LogInSystemAccountModel model)
        {
            

            var pwd = model.Password.ToSHA1();
            var account = await _db.SystemAccounts.FindOneAsync(f => f.Email == model.Email && f.Password == pwd);
            if (account == null)
            {
                return Ok(ApteryxResultApi.Fail(ApteryxCodes.账号或密码错误));
            }

            var role = await _db.Roles.FindOneAsync(f => f.Id == account.RoleId);

            var token = new JwtBuilder()
                .AddAudience(_jwtConfig.TokenConfig.Audience)
                .AddClaim(ClaimTypes.Name, account.Id)
                .AddSubject(Guid.NewGuid().ToString())
                .AddExpiry(_jwtConfig.TokenConfig.Expires)
                .AddIssuer(_jwtConfig.TokenConfig.Issuer)
                .AddSecurityKey(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.TokenConfig.Key)))
                .Build();
            if (_jwtConfig.IsSecurityToken)
            {
                var aesConfig = _jwtConfig.AESConfig;
                return Ok(ApteryxResultApi.Susuccessful(new Jwt<ResultSystemAccountRoleModel>(token, aesConfig.Key, aesConfig.IV, new ResultSystemAccountRoleModel(account, role))));
            }
            return Ok(ApteryxResultApi.Susuccessful(new Jwt<ResultSystemAccountRoleModel>(token, new ResultSystemAccountRoleModel(account, role))));
        }

        /// <summary>
        /// 添加账户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [SwaggerOperation(
            Summary = "添加账户",
            OperationId = "Post",
            Tags = new[] { "SystemAccount" }
        )]
        [ApiRoleDescription("A", "添加")]
        [SwaggerResponse((int)ApteryxCodes.Unauthorized, null, typeof(ApteryxResult))]
        public async Task<IActionResult> Post([FromBody] AddSystemAccountModel model)
        {
            var email = model.Email?.Trim();
            var pwd = model.Password.Trim();


            var check = await _db.SystemAccounts.FindOneAsync(f => f.Email == email);
            if (check != null)
                return Ok(ApteryxResultApi.Fail(ApteryxCodes.邮箱已被注册, "已存在该邮箱的账户"));


            var role = await _db.Roles.FindOneAsync(f => f.Id == model.RoleId);
            if (role == null)
                return Ok(ApteryxResultApi.Fail(ApteryxCodes.角色不存在, "该角色不存在"));

            await _db.SystemAccounts.AddAsync(new SystemAccount()
            {
                Name = model.Name,
                Email = email,
                Password = pwd.ToSHA1(),
                RoleId = model.RoleId
            });
            return Ok(ApteryxResultApi.Susuccessful());
        }

        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "获取指定账户信息",
            OperationId = "Get",
            Tags = new[] { "SystemAccount" }
        )]
        [ApiRoleDescription("B", "获取")]
        [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult<ResultSystemAccountRoleModel>))]
        [SwaggerResponse((int)ApteryxCodes.Unauthorized, null, typeof(ApteryxResult))]
        public async Task<IActionResult> Get([SwaggerParameter("账户ID", Required = false)] string id)
        {
            var account = await _db.SystemAccounts.FindOneAsync(f => f.Id == id);
            if (account == null)
                return Ok(ApteryxResultApi.Fail(ApteryxCodes.账户不存在));
            var role = _db.Roles.FindOne(f => f.Id == account.RoleId);
            return Ok(ApteryxResultApi.Susuccessful(new ResultSystemAccountRoleModel(account, role)));
        }

        [HttpPut]
        [SwaggerOperation(
            Summary = "修改账户与密码",
            OperationId = "Put",
            Tags = new[] { "SystemAccount" }
        )]
        [ApiRoleDescription("C", "修改账户与密码", isMustHave: true)]
        [SwaggerResponse((int)ApteryxCodes.Unauthorized, null, typeof(ApteryxResult))]
        public async Task<IActionResult> EditAccountPwd([FromBody] EditPwdSystemAccountModel model)
        {
            var accountId = HttpContext.GetAccountId();

            var oldpwd = model.OldPassword.ToSHA1();
            var account = await _db.SystemAccounts.FindOneAsync(f => f.Id == accountId && f.Email == model.OldEmail && f.Password == oldpwd);
            if (account == null)
            {
                return Ok(ApteryxResultApi.Fail(ApteryxCodes.账号或密码错误));
            }

            var check = await _db.SystemAccounts.FindOneAsync(f => f.Email == model.NewEmail.Trim() && f.Id != account.Id);
            if (check != null)
                return Ok(ApteryxResultApi.Fail(ApteryxCodes.邮箱已被注册, "已存在该邮箱的账户"));

            var result = await _db.SystemAccounts.WhereUpdateOneAsync(u => u.Id == account.Id, Builders<SystemAccount>
                .Update
                .Set(s => s.Email, model.NewEmail)
                .Set(s => s.Password, model.NewPassword.ToSHA1()));
            if (result.ModifiedCount <= 0)
                return Ok(ApteryxResultApi.Susuccessful("没有任何变更"));

            await _db.Logs.InsertOneAsync(new Log(accountId, "SystemAccount", ActionMethods.改, "修改账户与密码", account.ToJson(), _db.SystemAccounts.FindOne(f => f.Id == account.Id).ToJson()));
            return Ok(ApteryxResultApi.Susuccessful());
        }

        [HttpPost("query")]
        [SwaggerOperation(
            Summary = "查询",
            OperationId = "Query",
            Tags = new[] { "SystemAccount" }
        )]
        [ApiRoleDescription("D", "查询")]
        [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult<PageList<SystemAccount>>))]
        [SwaggerResponse((int)ApteryxCodes.Unauthorized, null, typeof(ApteryxResult))]
        public async Task<IActionResult> PostQuery([FromBody] QuerySystemAccountModel model)
        {
            var pindex = model.Page;
            var rowCount = model.Limit;

            FilterDefinition<SystemAccount> filter = null;
            var bf = Builders<SystemAccount>.Filter;

            if (!model.Email.IsNullOrWhiteSpace())
                filter = bf.Regex(r => r.Email, new BsonRegularExpression(model.Email));

            if (!model.RoleId.IsNullOrWhiteSpace())
                filter = bf.Eq(r => r.RoleId, model.RoleId);

            filter = filter ?? bf.Empty;

            var count = await _db.SystemAccounts.CountDocumentsAsync(filter);
            var item = _db.SystemAccounts.Find(filter).Sort(Builders<SystemAccount>.Sort.Descending(d => d.CreateTime)).Skip((pindex - 1) * rowCount).Limit(rowCount).ToList();

            return Ok(ApteryxResultApi.Susuccessful(new PageList<SystemAccount>(count, item)));
        }
    }
}
