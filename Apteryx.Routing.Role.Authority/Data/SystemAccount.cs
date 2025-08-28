using Apteryx.MongoDB.Driver.Extend;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Apteryx.Routing.Role.Authority;

public class SystemAccount : BaseMongoEntity
{
    /// <summary>
    /// 手机
    /// </summary>
    public required string Phone { get; set; }
    /// <summary>
    /// 姓名
    /// </summary>
    public required string Name { get; set; }
    /// <summary>
    /// 邮箱
    /// </summary>
    public required string Email { get; set; }
    /// <summary>
    /// 密码
    /// </summary>
    [JsonIgnore]
    public string Password { get; set; }
    /// <summary>
    /// 角色ID
    /// </summary>
    [BsonRepresentation(BsonType.ObjectId)]
    public required string RoleId { get; set; }
    /// <summary>
    /// 是否超级管理员
    /// </summary>
    [JsonIgnore]
    public bool IsSuper { get; set; } = false;
    /// <summary>
    /// 状态
    /// </summary>
    public bool State { get; set; } = true;
}
