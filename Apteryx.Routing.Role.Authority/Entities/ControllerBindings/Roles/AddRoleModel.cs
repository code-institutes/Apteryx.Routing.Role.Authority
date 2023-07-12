using System.ComponentModel.DataAnnotations;

namespace Apteryx.Routing.Role.Authority
{
    /// <summary>
    /// 添加角色模型
    /// </summary>
    public class AddRoleModel
    {
        /// <summary>
        /// 角色名
        /// </summary>
        [Required(ErrorMessage ="“角色名”必填")]
        public string? Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 路由ID
        /// </summary>
        [RequiredItem(ErrorMessage = "至少选中一个权限")]
        public required IEnumerable<string> RouteIds { get; set; }
    }
}
