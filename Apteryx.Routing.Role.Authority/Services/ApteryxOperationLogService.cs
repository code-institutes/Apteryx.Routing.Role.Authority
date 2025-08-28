using apteryx.common.extend.Helpers;
using Apteryx.MongoDB.Driver.Extend;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;

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

        public async Task CreateAsync<T>(T? newData, T? oldData, string? remarks = null, string? operationRecordText = null)
            where T : BaseMongoEntity
        {
            var traceIdentifier = _httpContext.HttpContext?.TraceIdentifier;
            var systemAccount = await _db.ApteryxSystemAccount.FindOneAsync(_httpContext.HttpContext?.GetAccountId());
            var callLog = await _db.ApteryxCallLog.FindOneAsync(f => f.TraceIdentifier == traceIdentifier);
            if (callLog == null) { return; }

            var actDesc = callLog.ActionDescriptor;
            var log = new OperationLog(
                    callLog.TraceIdentifier,
                    actDesc.ActionDescriptorId,
                    actDesc.GroupName,
                    actDesc.ControllerFullName,
                    actDesc.ControllerName,
                    actDesc.ActionName,
                    actDesc.ActionDescription,
                    callLog.Request.Method,
                    actDesc.Template,
                    remarks,
                    operationRecordText,
                    systemAccount,
                    newData?.Id ?? oldData?.Id,
                    typeof(T).FullName,
                    newData?.ToJson(),
                    oldData?.ToJson());
            log.DataId = newData?.Id ?? oldData?.Id;
            await _db.ApteryxOperationLog.AddAsync(log);
        }
        public async Task CreateAsync<T>(IClientSessionHandle clientSession, T? newData, T? oldData, string? remarks = null, string? operationRecordText = null)
            where T : BaseMongoEntity
        {
            var traceIdentifier = _httpContext.HttpContext?.TraceIdentifier;
            var systemAccount = await _db.ApteryxSystemAccount.FindOneAsync(_httpContext.HttpContext?.GetAccountId());
            var callLog = await _db.ApteryxCallLog.FindOneAsync(f => f.TraceIdentifier == traceIdentifier);
            if (callLog == null) { return; }

            var actDesc = callLog.ActionDescriptor;
            var log = new OperationLog(
                    callLog.TraceIdentifier,
                    actDesc.ActionDescriptorId,
                    actDesc.GroupName,
                    actDesc.ControllerFullName,
                    actDesc.ControllerName,
                    actDesc.ActionName,
                    actDesc.ActionDescription,
                    callLog.Request.Method,
                    actDesc.Template,
                    remarks,
                    operationRecordText,
                    systemAccount,
                    newData?.Id ?? oldData?.Id,
                    typeof(T).FullName,
                    newData?.ToJson(),
                    oldData?.ToJson());
            await _db.ApteryxOperationLog.AddAsync(clientSession, log);
        }

        public async Task<IApteryxResult> GetAsync(string id)
        {
            var log = await _db.ApteryxOperationLog.FindOneAsync(id);
            if (log == null)
                return ApteryxResultApi.Fail(ApteryxCodes.操作日志不存在);
            return ApteryxResultApi.Susuccessful(log);
        }

        public async Task<List<OperationLog>> GetAllAsync(string dataId)
        {
            var logs = await _db.ApteryxOperationLog.Where(w => w.DataId == dataId).ToListAsync();
            return logs;
        }

        public async Task<IApteryxResult> Query(QueryOperationLogModel model)
        {
            var query = _db.ApteryxOperationLog.AsMongoCollection.AsQueryable().AsQueryable();
            if (model.GroupName != null && !model.GroupName.IsNullOrWhiteSpace())
                query = query.Where(w => w.GroupName == model.GroupName);

            if (model.GroupId != null && !model.GroupId.IsNullOrWhiteSpace())
                query = query.Where(query => query.GroupId == model.GroupId);

            if (model.DataId != null && !model.DataId.IsNullOrWhiteSpace())
                query = query.Where(w => w.DataId == model.DataId);

            if (model.AccountId != null && !model.AccountId.IsNullOrWhiteSpace())
                query = query.Where(w => w.SystemAccount.Id == model.AccountId);

            if (model.ActionDescription != null && !model.ActionDescription.IsNullOrWhiteSpace())
                query = query.Where(w => w.ActionDescription == model.ActionDescription);

            if (model.ActionMethod != null && !model.ActionMethod.IsNullOrWhiteSpace())
                query = query.Where(w => w.ActionMethod == model.ActionMethod);

            if (model.ActionName != null && !model.ActionName.IsNullOrWhiteSpace())
                query = query.Where(w => w.ActionName == model.ActionName);

            if (model.Remarks != null && !model.Remarks.IsNullOrWhiteSpace())
                query = query.Where(w => w.Remarks.Contains(model.Remarks));

            if (model.KeyWord != null && !model.KeyWord.IsNullOrWhiteSpace())
                query = query.Where(w => w.NewData.Contains(model.KeyWord) || w.OldData.Contains(model.KeyWord));

            var data = await query.ToPageListAsync(model.Page, model.Limit);

            return ApteryxResultApi.Susuccessful(data);
        }

        //public async Task CreateAsync<T>(T? newData, T? oldData, string? remarks = null) where T : BaseMongoEntity
        //{
        //    var traceIdentifier = _httpContext.HttpContext?.TraceIdentifier;
        //    var systemAccount = await _db.SystemAccounts.FindOneAsync(_httpContext.HttpContext?.GetAccountId());
        //    var callLog = await _db.CallLogs.FindOneAsync(f => f.TraceIdentifier == traceIdentifier);
        //    if (callLog == null) { return; }

        //    var actDesc = callLog.ActionDescriptor;

        //    await _db.Database.GetCollection<OperationLog<T>>("ApteryxOperationLog").InsertOneAsync(new OperationLog<T>(
        //            callLog.TraceIdentifier,
        //            actDesc.ActionDescriptorId,
        //            actDesc.GroupName,
        //            actDesc.ControllerFullName,
        //            actDesc.ControllerName,
        //            actDesc.ActionName,
        //            actDesc.ActionDescription,
        //            callLog.Request.Method,
        //            actDesc.Template,
        //            remarks,
        //            systemAccount,
        //            newData,
        //            oldData));
        //}

        //private object? GetValue(BsonValue value)
        //{
        //    switch (value.BsonType)
        //    {
        //        case BsonType.Null:
        //            return null;

        //        case BsonType.String:
        //            return value.ToString();

        //        case BsonType.Boolean:
        //            return (bool)value;
        //        case BsonType.Double:
        //            return (double)value;

        //        case BsonType.Decimal128:
        //            return (decimal)value;

        //        case BsonType.DateTime:
        //            return (DateTime)value;

        //        case BsonType.Int64:
        //            return (Int64)value;

        //        case BsonType.Int32:
        //            return (Int32)value;

        //        case BsonType.ObjectId:
        //            return value.ToString();

        //        default: return value;
        //    }
        //}
        //private (string, object?) GetNameValue(string name, BsonValue value)
        //{
        //    if (name == "_id")
        //    {
        //        return ("Id", GetValue(value));
        //    }
        //    else if (value.IsBsonDocument)
        //    {
        //        Dictionary<string, object?> rootDic = new Dictionary<string, object?>();
        //        var subDocs = value.AsBsonDocument;
        //        foreach (var subDoc in subDocs)
        //        {
        //            var result = GetNameValue(subDoc.Name, subDoc.Value);
        //            rootDic.Add(result.Item1, result.Item2);
        //        }
        //        return (name, rootDic);
        //    }
        //    else if (value.IsBsonArray)
        //    {
        //        List<object?> rootList = new List<object?>();
        //        foreach (var item in value.AsBsonArray)
        //        {
        //            if (item.IsBsonDocument)
        //            {
        //                var subDocs = item.AsBsonDocument;
        //                Dictionary<string, object?> subDic = new Dictionary<string, object?>();
        //                foreach (var subDoc in subDocs)
        //                {
        //                    var result = GetNameValue(subDoc.Name, subDoc.Value);
        //                    subDic.Add(result.Item1, result.Item2);
        //                }
        //                rootList.Add(subDic);
        //            }
        //            else
        //            {
        //                rootList.Add(GetValue(item));
        //            }
        //        }

        //        return (name, rootList);
        //    }
        //    else
        //    {
        //        return (name, GetValue(value));
        //    }
        //}

        //public async Task<IApteryxResult> GetAsync(string id)
        //{
        //    var log = await _db.Database.GetCollection<OperationLog<BsonDocument>>("ApteryxOperationLog").FindOneAsync(f => f.Id == id);
        //    if (log == null)
        //        return ApteryxResultApi.Fail(ApteryxCodes.操作日志不存在);

        //    var type = Type.GetType(log.DataType);

        //    var result = new OperationLog<object>(log.TraceIdentifier, log.ActionDescriptorId, log.GroupName, log.ControllerFullName, log.ControllerName, log.ActionName, log.ActionDescription, log.ActionMethod, log.Template, log.Remarks, log.SystemAccount, null, null);
        //    if (log.NewData != null && log.OldData != null)
        //    {
        //        Dictionary<string, object?> oldDic = new Dictionary<string, object?>();
        //        foreach (var item in log.OldData)
        //        {
        //            if (item.Value.IsBsonNull)
        //            {
        //                oldDic.Add(item.Name, null);
        //                continue;
        //            }
        //           var nameValue =  GetNameValue(item.Name, item.Value);
        //            oldDic.Add(nameValue.Item1, nameValue.Item2);                    
        //        }
        //        var oldJson = oldDic.ToJson();
        //        result.OldData = Newtonsoft.Json.JsonConvert.DeserializeObject(oldJson, type);

        //        Dictionary<string,object?> newDic = new Dictionary<string, object?>();
        //        foreach(var item in log.NewData)
        //        {
        //            if(item.Value.IsBsonNull)
        //            {
        //                newDic.Add(item.Name, null);
        //                continue;
        //            }
        //            var nameValue = GetNameValue(item.Name,item.Value);
        //            newDic.Add(nameValue.Item1,nameValue.Item2);
        //        }
        //        var newJson = newDic.ToJson();
        //        result.NewData = Newtonsoft.Json.JsonConvert.DeserializeObject(newJson, type);

        //    }
        //    else if (log.NewData != null)
        //    {
        //        Dictionary<string, object?> newDic = new Dictionary<string, object?>();
        //        foreach (var item in log.NewData)
        //        {
        //            if (item.Value.IsBsonNull)
        //            {
        //                newDic.Add(item.Name, null);
        //                continue;
        //            }
        //            var nameValue = GetNameValue(item.Name, item.Value);
        //            newDic.Add(nameValue.Item1, nameValue.Item2);
        //        }
        //        var newJson = newDic.ToJson();
        //        result.NewData = Newtonsoft.Json.JsonConvert.DeserializeObject(newJson, type);
        //    }
        //    else
        //    {
        //        Dictionary<string, object?> oldDic = new Dictionary<string, object?>();
        //        foreach (var item in log.OldData)
        //        {
        //            if (item.Value.IsBsonNull)
        //            {
        //                oldDic.Add(item.Name, null);
        //                continue;
        //            }
        //            var nameValue = GetNameValue(item.Name, item.Value);
        //            oldDic.Add(nameValue.Item1, nameValue.Item2);
        //        }
        //        var oldJson = oldDic.ToJson();
        //        result.OldData = Newtonsoft.Json.JsonConvert.DeserializeObject(oldJson, type);
        //    }

        //    return ApteryxResultApi.Susuccessful(result);
        //}
    }
}
