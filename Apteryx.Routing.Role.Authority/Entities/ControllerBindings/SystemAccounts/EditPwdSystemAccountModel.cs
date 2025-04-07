using System.ComponentModel.DataAnnotations;

namespace Apteryx.Routing.Role.Authority
{
    /// <summary>
    /// 修改账户与密码模型
    /// </summary>
    public sealed class EditPwdSystemAccountModel
    {
        /// <summary>
        /// 原邮箱（账户）
        /// </summary>
        [EmailAddress]
        [Required(ErrorMessage = "旧邮箱必填")]
        public required string OldEmail { get; set; }

        /// <summary>
        /// 原密码
        /// </summary>
        [Required(ErrorMessage ="旧密码必填")]
        public required string OldPassword { get; set; }

        /// <summary>
        /// 新邮箱（账户）
        /// </summary>
        [EmailAddress]
        [Required(ErrorMessage = "新邮箱必填")]
        public required string NewEmail { get; set; }

        /// <summary>
        /// 新密码
        /// </summary>
        [Required(ErrorMessage ="新密码必填")]
        public required string NewPassword { get; set; }
    }
}
