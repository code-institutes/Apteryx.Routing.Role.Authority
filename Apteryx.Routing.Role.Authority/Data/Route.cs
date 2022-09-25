using Apteryx.MongoDB.Driver.Extend;

namespace Apteryx.Routing.Role.Authority
{
    public class Route:BaseMongoEntity
    {
        public string CtrlName { get; set; }
        public string Method { get; set; }
        public string Path { get; set; }
        public AddTypes AddType { get; set; } = AddTypes.人工;
        public string Tag { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsMustHave { get; set; }
        public string CtrlFullName { get; set; }
    }
}
