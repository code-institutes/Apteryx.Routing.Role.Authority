using System.ComponentModel.DataAnnotations;

namespace Apteryx.Routing.Role.Authority
{
    /// <summary>
    /// 添加系统账户模型
    /// </summary>
    public class AddSystemAccountModel
    {
        /// <summary>
        /// 姓名
        /// </summary>
        [Required(ErrorMessage = "姓名必填")]
        public string? Name { get; set; }

        /// <summary>
        /// 账户
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
        /// 角色ID
        /// </summary>
        [Required(ErrorMessage ="“角色必填")]
        public string? RoleId { get; set; }
    }
}
