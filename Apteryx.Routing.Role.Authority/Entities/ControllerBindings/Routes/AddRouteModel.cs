using System.ComponentModel.DataAnnotations;

namespace Apteryx.Routing.Role.Authority
{
    /// <summary>
    /// 添加路由模型
    /// </summary>
    public class AddRouteModel
    {
        /// <summary>
        /// 标签
        /// </summary>
        [Required(ErrorMessage = "“{0}”必填")]
        public required string Tag { get; set; }
        /// <summary>
        /// 接口名
        /// </summary>
        [Required(ErrorMessage = "“{0}”必填")]
        public required string Name { get; set; }
        /// <summary>
        /// 功能说明
        /// </summary>
        [Required(ErrorMessage ="“{0}”必填")]
        public required string CtrlName { get; set; }

        /// <summary>
        /// 行为
        /// </summary>
        [Required(ErrorMessage ="“{0}”必填")]
        public required string Method { get; set; }

        /// <summary>
        /// 接口描述
        /// </summary>
        [Required(ErrorMessage ="“{0}”必填")]
        public required string Description { get; set; }

        /// <summary>
        /// 相对路径
        /// </summary>
        [Required(ErrorMessage ="“{0}”必填")]
        public required string Path { get; set; }
    }
}
