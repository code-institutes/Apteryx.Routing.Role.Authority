using Apteryx.MongoDB.Driver.Extend;

namespace Apteryx.Routing.Role.Authority
{
    /// <summary>
    /// 日志实体模型
    /// </summary>
    public class OperationLog : BaseMongoEntity
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
        /// <param name="newData"></param>
        /// <param name="oldData"></param>
        public OperationLog(
            string traceIdentifier,
            string? actionDescriptorId,
            string? groupName,
            string? controllerFullName,
            string? controllerName,
            string? actionName,
            string? actionDescription,
            string? actionMethod,
            string? template,
            string? remarks,
            SystemAccount? systemAccount,
            string? newData,
            string? oldData) =>
            (TraceIdentifier, ActionDescriptorId, GroupName, ControllerFullName, ControllerName, ActionName, ActionDescription, ActionMethod, Template, Remarks, SystemAccount, NewData, OldData) =
            (traceIdentifier, actionDescriptorId, groupName, controllerFullName, controllerName, actionName, actionDescription, actionMethod, template, remarks, systemAccount, newData, oldData);

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
        public string? ActionDescriptorId { get; set; }
        /// <summary>
        /// 组名
        /// </summary>
        public string? GroupName { get; set; }
        /// <summary>
        /// 控制器全名
        /// </summary>
        public string? ControllerFullName { get; set; }
        /// <summary>
        /// 控制器名称
        /// </summary>
        public string? ControllerName { get; set; }
        /// <summary>
        /// 操作名
        /// </summary>
        public string? ActionName { get; set; }
        /// <summary>
        /// 操作摘要
        /// </summary>
        public string? ActionDescription { get; set; }
        /// <summary>
        /// 方法协议
        /// </summary>
        public string? ActionMethod { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? Template { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string? Remarks { get; set; }
        /// <summary>
        /// 操作人员类型
        /// </summary>
        public SystemAccount? SystemAccount { get; set; }
        ///// <summary>
        ///// 数据类型
        ///// </summary>
        //public string DataType { get; set; } = typeof(T).FullName;
        /// <summary>
        /// 
        /// </summary>
        public string? NewData { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? OldData { get; set; }
    }

    ///// <summary>
    ///// 日志实体模型
    ///// </summary>
    //public class OperationLog<T> : BaseMongoEntity
    //{
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="traceIdentifier"></param>
    //    /// <param name="actionDescriptorId"></param>
    //    /// <param name="groupName"></param>
    //    /// <param name="controllerFullName"></param>
    //    /// <param name="controllerName"></param>
    //    /// <param name="actionName"></param>
    //    /// <param name="actionDescription"></param>
    //    /// <param name="actionMethod"></param>
    //    /// <param name="template"></param>
    //    /// <param name="newData"></param>
    //    /// <param name="oldData"></param>
    //    public OperationLog(
    //        string traceIdentifier,
    //        string? actionDescriptorId,
    //        string? groupName,
    //        string? controllerFullName,
    //        string? controllerName,
    //        string? actionName,
    //        string? actionDescription,
    //        string? actionMethod,
    //        string? template,
    //        string? remarks,
    //        SystemAccount? systemAccount,
    //        T? newData,
    //        T? oldData) =>
    //        (TraceIdentifier, ActionDescriptorId, GroupName, ControllerFullName, ControllerName, ActionName, ActionDescription, ActionMethod, Template, Remarks,SystemAccount, NewData, OldData) =
    //        (traceIdentifier, actionDescriptorId, groupName, controllerFullName, controllerName, actionName, actionDescription, actionMethod, template, remarks,systemAccount, newData, oldData);

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public string? GroupId { get; set; }
    //    /// <summary>
    //    /// 跟踪标识
    //    /// </summary>
    //    public string TraceIdentifier { get; set; }
    //    /// <summary>
    //    /// 动作ID
    //    /// </summary>
    //    public string? ActionDescriptorId { get; set; }
    //    /// <summary>
    //    /// 组名
    //    /// </summary>
    //    public string? GroupName { get; set; }
    //    /// <summary>
    //    /// 控制器全名
    //    /// </summary>
    //    public string? ControllerFullName { get; set; }
    //    /// <summary>
    //    /// 控制器名称
    //    /// </summary>
    //    public string? ControllerName { get; set; }
    //    /// <summary>
    //    /// 操作名
    //    /// </summary>
    //    public string? ActionName { get; set; }
    //    /// <summary>
    //    /// 操作摘要
    //    /// </summary>
    //    public string? ActionDescription { get; set; }
    //    /// <summary>
    //    /// 方法协议
    //    /// </summary>
    //    public string? ActionMethod { get; set; }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public string? SystemAccountId { get; set; }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public string? Template { get; set; }
    //    /// <summary>
    //    /// 备注
    //    /// </summary>
    //    public string? Remarks { get; set; }
    //    /// <summary>
    //    /// 操作人员类型
    //    /// </summary>
    //    public SystemAccount? SystemAccount { get; set; }
    //    /// <summary>
    //    /// 数据类型
    //    /// </summary>
    //    public string DataType { get; set; } = typeof(T).FullName;
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public T? NewData { get; set; }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public T? OldData { get; set; }
    //}
}
