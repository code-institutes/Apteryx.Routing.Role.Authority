using Apteryx.MongoDB.Driver.Extend;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Swashbuckle.AspNetCore.Annotations;
using MongoDB.Driver;
using apteryx.common.extend.Helpers;
using MongoDB.Bson;
using Apteryx.Routing.Role.Authority.Services;
using Microsoft.Extensions.Caching.Distributed;

namespace Apteryx.Routing.Role.Authority.Controllers
{
    [SwaggerTag("验证码服务")]
    [Route("apteryx/captcha")]
    [Produces("application/json")]
    [ApiExplorerSettings(GroupName = "apteryx1.0")]
    [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult))]
    public class CaptchaController : ControllerBase
    {
        private readonly ApteryxOperationLogService _log;
        private readonly IDistributedCache _cache;

        public CaptchaController(ApteryxOperationLogService logService, IDistributedCache cache)
        {
            this._log = logService;
            this._cache = cache;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "获取指定日志信息",
            OperationId = "Get",
            Tags = new[] { "Captcha" }
        )]
        [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult<byte[]>))]
        public async Task<IActionResult> Get(
            [SwaggerParameter("邮箱/手机", Required = true)] string key,
            [SwaggerParameter("类型", Required = true)] CaptchaType type)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return Ok(ApteryxResultApi.Fail(ApteryxCodes.字段验证未通过,"必须提供手机号或邮箱作为 key"));
            }

            var captchaService = new CaptchaService(_cache);
            var (code, imageBytes) = await captchaService.GenerateCaptchaAsync(key, type);

            // 将 byte[] 转为 Base64 字符串
            string base64Image = Convert.ToBase64String(imageBytes);
            // 如果需要，将 Base64 字符串前加上 data URI 前缀，便于前端直接使用
            string imageDataUrl = $"data:image/png;base64,{base64Image}";
            return Ok(ApteryxResultApi.Susuccessful(imageDataUrl));
        }
        
        [HttpGet("check-captcha")]
        [SwaggerOperation(
            Summary = "检验验证码",
            Description = "检验验证码是否正确",
            OperationId = "CheckCaptcha",
            Tags = new[] { "Captcha" }
        )]
        [SwaggerResponse((int)ApteryxCodes.验证码错误, null, typeof(ApteryxResult))]
        [SwaggerResponse((int)ApteryxCodes.字段验证未通过, null, typeof(ApteryxResult<List<FieldValid>>))]
        public async Task<IActionResult> CheckCaptcha(
            [SwaggerParameter("邮箱/手机", Required = true)] string? key,
            [SwaggerParameter("行为类型", Required = true)] CaptchaType? type,
            [SwaggerParameter("验证码", Required = true)] string? code)
        {
            if (string.IsNullOrWhiteSpace(key))
                return Ok(ApteryxResultApi.Fail(ApteryxCodes.字段验证未通过, "邮箱/手机不能为空"));
            if (string.IsNullOrWhiteSpace(code))
                return Ok(ApteryxResultApi.Fail(ApteryxCodes.字段验证未通过, "验证码不能为空"));
            if (type == null)
                return Ok(ApteryxResultApi.Fail(ApteryxCodes.字段验证未通过, "行为类型不能为空"));

            var cachedCode = await _cache.GetStringAsync($"Captcha_{type}_{key}");
            if (string.IsNullOrEmpty(cachedCode) || cachedCode != code.ToUpper())
            {
                return Ok(ApteryxResultApi.Fail(ApteryxCodes.验证码错误));
            }
            return Ok(ApteryxResultApi.Susuccessful());
        }
    }
}
