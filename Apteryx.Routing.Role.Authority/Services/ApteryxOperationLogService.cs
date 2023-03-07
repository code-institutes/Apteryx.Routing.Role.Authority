using Apteryx.MongoDB.Driver.Extend;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text.Json;

namespace Apteryx.Routing.Role.Authority
{
    public class ApteryxOperationLogService
    {
        private readonly ApteryxDbContext _db;
        private readonly IHttpContextAccessor _httpContext;
        public ApteryxOperationLogService(ApteryxDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            this._db = db;
            this._httpContext = httpContextAccessor;
        }
        public async Task CreateAsync<T>(T? newData, T? oldData) where T : BaseMongoEntity
        {
            var traceIdentifier = _httpContext.HttpContext?.TraceIdentifier;
            var callLog = await _db.CallLogs.FindOneAsync(f => f.TraceIdentifier == traceIdentifier);
            if (callLog == null) { return; }

            var actDesc = callLog.ActionDescriptor;

            await _db.Database.GetCollection<OperationLog<T>>("ApteryxOperationLog").InsertOneAsync(new OperationLog<T>(
                    callLog.TraceIdentifier,
                    actDesc.ActionDescriptorId,
                    actDesc.GroupName,
                    actDesc.ControllerFullName,
                    actDesc.ControllerName,
                    actDesc.ActionName,
                    actDesc.ActionDescription,
                    callLog.Request.Method,
                    actDesc.Template,
                    newData,
                    oldData));
        }

        public async Task<IApteryxResult> GetAsync(string id)
        {
            var log = await _db.Database.GetCollection<OperationLog<BsonDocument>>("ApteryxOperationLog").FindOneAsync(f => f.Id == id);
            if (log == null)
                return ApteryxResultApi.Fail(ApteryxCodes.操作日志不存在);

            var type = Type.GetType(log.DataType);

            var result = new OperationLog<object>(log.TraceIdentifier, log.ActionDescriptorId, log.GroupName, log.ControllerFullName, log.ControllerName, log.ActionName, log.ActionDescription, log.ActionMethod, log.Template, null, null);
            if (log.NewData != null && log.OldData != null)
            {
                var oldDic = log.OldData.ToDictionary(f => f.Name, f => f.Value?.ToString());
                var oldJson = oldDic.ToJson().Replace("\"_id\"", "\"Id\"").Replace("\"BsonNull\"","null");
                result.OldData = Newtonsoft.Json.JsonConvert.DeserializeObject(oldJson, type);

                var newDic = log.NewData.ToDictionary(f => f.Name, f => f.Value?.ToString());
                var newJson = newDic.ToJson().Replace("\"_id\"", "\"Id\"").Replace("\"BsonNull\"", "null");
                result.NewData = Newtonsoft.Json.JsonConvert.DeserializeObject(newJson, type);

            }
            else if (log.NewData != null)
            {
                var newDic = log.NewData.ToDictionary(f => f.Name, f => f.Value?.ToString());
                var newJson = newDic.ToJson().Replace("\"_id\"", "\"Id\"").Replace("\"BsonNull\"", "null");
                result.NewData = Newtonsoft.Json.JsonConvert.DeserializeObject(newJson, type);
            }
            else
            {
                var oldDic = log.OldData.ToDictionary(f => f.Name, f => f.Value?.ToString());
                var oldJson = oldDic.ToJson().Replace("\"_id\"", "\"Id\"").Replace("\"BsonNull\"", "null");
                result.OldData = Newtonsoft.Json.JsonConvert.DeserializeObject(oldJson, type);
            }

            return ApteryxResultApi.Susuccessful(result);
        }
    }
}
