using Apteryx.MongoDB.Driver.Extend;

namespace Apteryx.Routing.Role.Authority
{
    public class CallLog : BaseMongoEntity
    {
        public string TraceIdentifier { get; set; }
        public string? IdentityName { get; set; }
        public SystemAccount? SystemAccount { get; set; }
        public ActionDescriptor ActionDescriptor { get; set; }
        public Connection Connection { get; set; }
        public Request Request { get; set; }
        public Response? Response { get; set; }
    }

    public sealed class Connection
    {
        public string? ConnectionId { get; set; }
        public string? RemoteIpAddress { get; set; }
        public int? RemotePort { get; set; }
    }

    public sealed class ActionDescriptor
    {
        public string GroupName { get; set; }
        public string? ActionDescriptorId { get; set; }
        public string? ControllerFullName { get; set; }
        public string? ControllerName { get; set; }
        public string? ActionName { get; set; }
        public string? ActionDescription { get; set; }
        public string? Template { get; set; }
    }

    public sealed class Request
    {
        public string? ContentType { get; set; }
        public long? ContentLength { get; set; }
        public string? QueryString { get; set; }
        public string? Scheme { get; set; }
        public string? Protocol { get; set; }
        public string? Method { get; set; }
        public string? Path { get; set; }
        public Dictionary<string, string>? Heads { get; set; }
        public bool ModelState { get; set; } = true;
        public IEnumerable<string>? ModelError { get; set; }
        public IEnumerable<Body>? Bodys { get; set; }
        public DateTime CreateTime { get; set; } = DateTime.Now;
    }
    public sealed class Body
    {
        /// <summary>
        /// 模型名称
        /// </summary>
        public string? ModelName { get; set; }
        /// <summary>
        /// 报文
        /// </summary>
        public string? Payload { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string? Type { get; set; }
    }

    public class Response
    {
        public int StatusCode { get; set; }
        public string? Result { get; set; }
        public string? Type { get; set; }
        public DateTime CreateTime { get; set; } = DateTime.Now;
    }
}
