using Apteryx.MongoDB.Driver.Extend;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Apteryx.Routing.Role.Authority
{
    public sealed class Log : BaseMongoEntity
    {
        public Log(string systemAccountId, string mongoCollectionName, ActionMethods actionMethod, string actionName, string? source = null, string? after = null, string? groupId = null)
        {
            this.SystemAccountId = systemAccountId;
            this.MongoCollectionName = mongoCollectionName;
            this.ActionMethod = actionMethod;
            this.ActionName = actionName;
            this.Source = source;
            this.After = after;
            this.GroupId = groupId;
        }
        /// <summary>
        /// 事件组ID
        /// </summary>
        [BsonRepresentation(BsonType.ObjectId)]
        public string? GroupId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [BsonRepresentation(BsonType.ObjectId)]
        public string SystemAccountId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MongoCollectionName { get; set; }
        /// <summary>
        /// 原
        /// </summary>
        public string? Source { get; set; }
        /// <summary>
        /// 之后
        /// </summary>
        public string? After { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ActionMethods ActionMethod { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ActionName { get; set; }
    }
}
