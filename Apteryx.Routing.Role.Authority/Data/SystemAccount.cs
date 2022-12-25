using Apteryx.MongoDB.Driver.Extend;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Apteryx.Routing.Role.Authority
{
    public class SystemAccount:BaseMongoEntity
    {
        /// <summary>
        /// 
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [JsonIgnore]
        public string Password { get; set; }
        /// <summary>
        /// 角色ID
        /// </summary>
        [BsonRepresentation(BsonType.ObjectId)]
        public string RoleId { get; set; }
        /// <summary>
        /// 是否超级管理员
        /// </summary>
        [JsonIgnore]
        public bool IsSuper { get; set; } = false;
    }
}
