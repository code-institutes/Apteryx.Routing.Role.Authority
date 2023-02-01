using System.ComponentModel.DataAnnotations;

namespace Apteryx.Routing.Role.Authority
{
    /// <summary>
    /// 修改账户状态模型
    /// </summary>
    public class EditStateSystemAccountModel
    {
        /// <summary>
        /// 账户ID
        /// </summary>
        [Required(ErrorMessage = "“{0}”必填")]
        public string? Id { get; set; }
    }
}
