using apteryx.common.extend.Helpers;
using Apteryx.MongoDB.Driver.Extend;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using MongoDB.Bson;
using MongoDB.Driver;
using Swashbuckle.AspNetCore.Annotations;
using System.Reflection;

namespace Apteryx.Routing.Role.Authority;

public class ApteryxInitializeDataService
{
    private readonly ApteryxDbContext _db;
    private readonly IActionDescriptorCollectionProvider actionDescriptor;
    public ApteryxInitializeDataService(IActionDescriptorCollectionProvider collectionProvider, ApteryxDbContext dbContext)
    {
        this._db = dbContext;
        this.actionDescriptor = collectionProvider;

        //创建表索引
        _db.ApteryxCallLog.Native.Indexes.CreateOne(new CreateIndexModel<CallLog>(Builders<CallLog>.IndexKeys.Ascending(f => f.TraceIdentifier)));
        _db.ApteryxRoute.Native.Indexes.CreateOne(new CreateIndexModel<Route>(Builders<Route>.IndexKeys.Ascending(f => f.Path).Ascending(f => f.Method)));
        _db.ApteryxSystemAccount.Native.Indexes.CreateOne(new CreateIndexModel<SystemAccount>(Builders<SystemAccount>.IndexKeys.Ascending(f => f.Email).Ascending(f => f.Password)));

        _db.ApteryxOperationLog.Native.Indexes.CreateOne(new CreateIndexModel<OperationLog>(Builders<OperationLog>.IndexKeys.Ascending(f => f.GroupId)));
        _db.ApteryxOperationLog.Native.Indexes.CreateOne(new CreateIndexModel<OperationLog>(Builders<OperationLog>.IndexKeys.Ascending(f => f.DataId)));
        _db.ApteryxOperationLog.Native.Indexes.CreateOne(new CreateIndexModel<OperationLog>(Builders<OperationLog>.IndexKeys.Ascending(f => f.GroupName)));
        _db.ApteryxOperationLog.Native.Indexes.CreateOne(new CreateIndexModel<OperationLog>(Builders<OperationLog>.IndexKeys.Ascending(f => f.Remarks)));
        _db.ApteryxOperationLog.Native.Indexes.CreateOne(new CreateIndexModel<OperationLog>(Builders<OperationLog>.IndexKeys.Ascending(f => f.ActionMethod)));
        _db.ApteryxOperationLog.Native.Indexes.CreateOne(new CreateIndexModel<OperationLog>(Builders<OperationLog>.IndexKeys.Ascending(f => f.ActionName)));
        _db.ApteryxOperationLog.Native.Indexes.CreateOne(new CreateIndexModel<OperationLog>(Builders<OperationLog>.IndexKeys.Ascending(f => f.ActionDescription)));
        _db.ApteryxOperationLog.Native.Indexes.CreateOne(new CreateIndexModel<OperationLog>(Builders<OperationLog>.IndexKeys.Ascending(f => f.SystemAccount.Id)));
        _db.ApteryxOperationLog.Native.Indexes.CreateOne(new CreateIndexModel<OperationLog>(Builders<OperationLog>.IndexKeys.Ascending(f => f.ControllerName)));
    }

    /// <summary>
    /// 初始化账户与角色
    /// </summary>
    public void InitAccountRole()
    {
        //刷新路由
        RefreshRoute();

        var route = _db.ApteryxRoute.Immediate.FindAll();

        var routeIds = route.Select(r => r.Id).ToList();

        //创建账户
        var act = _db.ApteryxSystemAccount.Immediate.FindAll();
        if (!act.Any())
        {
            var role = _db.ApteryxRole.Immediate.FindOne(f => f.Name == "管理员" && f.AddType == AddTypes.程序);
            if (role == null)
            {


                role = new Role()
                {
                    Name = "管理员",
                    AddType = AddTypes.程序,
                    Description = "系统默认管理员（非超管）",
                    Id = ObjectId.GenerateNewId().ToString(),
                    RouteIds = routeIds
                };
                _db.ApteryxRole.Immediate.Insert(role);
            }

            _db.ApteryxSystemAccount.Immediate.Insert(new SystemAccount()
            {
                Phone = "18611515159",
                Name = "super admin",
                Email = "wyspaces@outlook.com",
                Password = "admin1234".ToSHA1(),
                IsSuper = true,
                RoleId = role.Id
            });
        }
        else
        {
            var role = _db.ApteryxRole.Immediate.FindOne(f => f.Name == "管理员" && f.AddType == AddTypes.程序 || f.Name == "超管" && f.AddType == AddTypes.程序);
            _db.ApteryxRole.Immediate.UpdateOne(u => u.Id == role.Id, Builders<Role>.Update.Set(s => s.RouteIds, routeIds));
        }
    }

    //刷新路由
    public void RefreshRoute()
    {
        var serviceName = Assembly.GetEntryAssembly()?.GetName().Name;
        var meServiceName = "Apteryx.Routing.Role.Authority";
        List<Route> arrRoutes = new List<Route>();
        foreach (var action in actionDescriptor.ActionDescriptors.Items)
        {
            var ctrlFullName = string.Empty;
            var groupName = string.Empty;
            var actFullName = string.Empty;
            var httpMethods = "GET,POST,PUT,DELETE,PATCH";

            var ctrlActDesc = (ControllerActionDescriptor)action;

            if (action.ActionConstraints != null && action.ActionConstraints.Any())
            {
                var methodActionConstraint = action.ActionConstraints.First(f => f.GetType() == typeof(HttpMethodActionConstraint)) as HttpMethodActionConstraint;
                if (methodActionConstraint == null)
                    continue;

                httpMethods = string.Join(',', methodActionConstraint.HttpMethods.Select(s => s));
            }

            ctrlFullName = ctrlActDesc.ControllerTypeInfo.FullName;
            actFullName = action.DisplayName;

            var ctrlSwaggerTagAttr = action.EndpointMetadata.FirstOrDefault(f => f.GetType() == typeof(SwaggerTagAttribute));
            if (ctrlSwaggerTagAttr != null)
                groupName = ((SwaggerTagAttribute)ctrlSwaggerTagAttr).Description;
            else
                groupName = ctrlActDesc.ControllerName;

            var apiRoleDescObject = ctrlActDesc.EndpointMetadata.FirstOrDefault(f => f.GetType() == typeof(ApiRoleDescriptionAttribute));

            if (apiRoleDescObject != null)
            {
                var apiRoleDesc = (ApiRoleDescriptionAttribute)apiRoleDescObject;

                arrRoutes.Add(new Route()
                {
                    ServiceName = ctrlFullName.Contains(meServiceName) ? meServiceName : serviceName,
                    CtrlFullName = ctrlFullName,
                    CtrlName = groupName,
                    Path = $"/{ctrlActDesc.AttributeRouteInfo.Template}",
                    Method = httpMethods,
                    AddType = AddTypes.程序,
                    Tag = apiRoleDesc.Tag,
                    Name = apiRoleDesc.Name,
                    Description = apiRoleDesc.Description,
                    IsMustHave = apiRoleDesc.IsMustHave
                });
            }
        }

        foreach (var route in _db.ApteryxRoute.Where(w => w.AddType == AddTypes.程序 && w.ServiceName == serviceName || w.AddType == AddTypes.程序 && w.ServiceName == meServiceName).ToList())
        {
            var validRoute = arrRoutes.FirstOrDefault(a => a.CtrlFullName == route.CtrlFullName && a.Tag == route.Tag);
            if (validRoute != null)
            {
                route.CtrlFullName = validRoute.CtrlFullName;
                route.Method = validRoute.Method;
                route.Name = validRoute.Name;
                route.Description = validRoute.Description;
                route.Path = validRoute.Path;
                _db.ApteryxRoute.Immediate.ReplaceOne(r => r.Id == route.Id, route);
                arrRoutes.Remove(validRoute);
            }
            else
            {
                _db.ApteryxRoute.Immediate.DeleteOne(d => d.Id == route.Id);
                //将路由从所有角色中删除
                _db.ApteryxRole.Immediate.UpdateMany(u => u.RouteIds.Contains(route.Id), Builders<Role>.Update.Pull(p => p.RouteIds, route.Id));
            }
        }

        if (arrRoutes.Any())
            _db.ApteryxRoute.Immediate.InsertMany(arrRoutes);
    }
}
