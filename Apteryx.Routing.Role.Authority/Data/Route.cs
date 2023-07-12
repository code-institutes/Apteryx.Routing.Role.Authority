using Apteryx.MongoDB.Driver.Extend;

namespace Apteryx.Routing.Role.Authority
{
    public class Route:BaseMongoEntity
    {
        public required string CtrlName { get; set; }
        public required string Method { get; set; }
        public required string Path { get; set; }
        public AddTypes AddType { get; set; } = AddTypes.人工;
        public required string Tag { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public bool IsMustHave { get; set; }
        public required string CtrlFullName { get; set; }
    }
}
