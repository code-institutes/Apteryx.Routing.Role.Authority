using Apteryx.MongoDB.Driver.Extend;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Apteryx.Routing.Role.Authority
{
    /// <summary>
    /// 操作日志
    /// </summary>
    public class Log : BaseMongoEntity
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="traceIdentifier"></param>
        /// <param name="actionDescriptorId"></param>
        /// <param name="groupName"></param>
        /// <param name="controllerFullName"></param>
        /// <param name="controllerName"></param>
        /// <param name="actionName"></param>
        /// <param name="actionDescription"></param>
        /// <param name="actionMethod"></param>
        /// <param name="template"></param>
        /// <param name="systemAccountId"></param>
        public Log(
            string traceIdentifier,
            string actionDescriptorId,
            string groupName,
            string controllerFullName,
            string controllerName,
            string actionName,
            string? actionDescription,
            string actionMethod,
            string template) =>
            (TraceIdentifier, ActionDescriptorId, GroupName, ControllerFullName, ControllerName, ActionName, ActionDescription, ActionMethod, Template) =
            (traceIdentifier, actionDescriptorId, groupName, controllerFullName, controllerName, actionName, actionDescription, actionMethod, template);

        /// <summary>
        /// 
        /// </summary>
        public string? GroupId { get; set; }
        /// <summary>
        /// 跟踪标识
        /// </summary>
        public string TraceIdentifier { get; set; }
        /// <summary>
        /// 动作ID
        /// </summary>
        public string ActionDescriptorId { get; set; }
        /// <summary>
        /// 组名
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// 控制器全名
        /// </summary>
        public string ControllerFullName { get; set; }
        /// <summary>
        /// 控制器名称
        /// </summary>
        public string ControllerName { get; set; }
        /// <summary>
        /// 操作名
        /// </summary>
        public string ActionName { get; set; }
        /// <summary>
        /// 操作摘要
        /// </summary>
        public string? ActionDescription { get; set; }
        /// <summary>
        /// 方法协议
        /// </summary>
        public string ActionMethod { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SystemAccountId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Template { get; set; }
    }

    /// <summary>
    /// 日志实体模型
    /// </summary>
    public class Log<T> : Log
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="traceIdentifier"></param>
        /// <param name="actionDescriptorId"></param>
        /// <param name="groupName"></param>
        /// <param name="controllerFullName"></param>
        /// <param name="controllerName"></param>
        /// <param name="actionName"></param>
        /// <param name="actionDescription"></param>
        /// <param name="actionMethod"></param>
        /// <param name="template"></param>
        /// <param name="systemAccountId"></param>
        /// <param name="groupId"></param>
        /// <param name="dataOld"></param>
        /// <param name="dataNew"></param>
        public Log(
            string traceIdentifier,
            string actionDescriptorId,
            string groupName,
            string controllerFullName,
            string controllerName,
            string actionName,
            string? actionDescription,
            string actionMethod,
            string template,
            string systemAccountId,
            string? groupId,
            T? dataOld,
            T? dataNew) : base(traceIdentifier, actionDescriptorId, groupName, controllerFullName, controllerName, actionName, actionDescription, actionMethod, template) =>
            (DataOld, DataNew) =
            (dataOld, dataNew);

        /// <summary>
        /// 
        /// </summary>
        public T? DataOld { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public T? DataNew { get; set; }
    }
}
