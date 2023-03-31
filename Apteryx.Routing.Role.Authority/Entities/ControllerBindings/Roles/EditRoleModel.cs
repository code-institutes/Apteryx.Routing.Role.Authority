using System.ComponentModel.DataAnnotations;

namespace Apteryx.Routing.Role.Authority
{
    /// <summary>
    /// 编辑角色模型
    /// </summary>
    public sealed class EditRoleModel:AddRoleModel
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        [Required(ErrorMessage ="“角色ID”必填")]
        public string? Id { get; set; }
    }
}
