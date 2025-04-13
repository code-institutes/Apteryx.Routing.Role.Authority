using System.ComponentModel.DataAnnotations;

namespace Apteryx.Routing.Role.Authority
{
    /// <summary>
    /// 登录模型
    /// </summary>
    public sealed class LogInSystemAccountModel
    {
        /// <summary>
        /// 邮箱
        /// </summary>
        [EmailAddress]
        [Required(ErrorMessage = "邮箱必填")]
        public string? Email { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [Required(ErrorMessage = "密码必填")]
        public string? Password { get; set; }

        /// <summary>
        /// 验证类型
        /// </summary>
        [Required(ErrorMessage = "验证类型必填")]
        public CaptchaType? CaptchaType { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        [Required(ErrorMessage = "验证码必填")]
        public string? CaptchaCode { get; set; }
        
    }
}
