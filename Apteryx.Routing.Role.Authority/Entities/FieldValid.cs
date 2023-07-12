namespace Apteryx.Routing.Role.Authority
{
    public sealed class FieldValid
    {
        /// <summary>
        /// 字段名
        /// </summary>
        public required string Field { get; set; }

        /// <summary>
        /// 错误描述
        /// </summary>
        public required IEnumerable<string> Error { get; set; }
    }
}
