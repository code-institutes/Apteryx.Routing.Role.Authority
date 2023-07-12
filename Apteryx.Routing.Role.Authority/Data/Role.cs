using Apteryx.MongoDB.Driver.Extend;

namespace Apteryx.Routing.Role.Authority
{
    public class Role:BaseMongoEntity
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required List<string> RouteIds { get; set; }
        public List<string>? OwnRoleIds { get; set; }
        public AddTypes AddType { get; set; } = AddTypes.人工;
    }
}
