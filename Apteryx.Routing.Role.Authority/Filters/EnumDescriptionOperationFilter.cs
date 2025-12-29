using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace Apteryx.Routing.Role.Authority.Filters;

public class EnumDescriptionOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters == null)
            return;

        // 1. 先构建一个字典：参数名 → 枚举说明
        var enumDescriptions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var apiParam in context.ApiDescription.ParameterDescriptions)
        {
            var type = apiParam.Type;

            // 可空枚举处理
            if (Nullable.GetUnderlyingType(type) is Type underlying)
                type = underlying;

            if (!type.IsEnum)
                continue;

            var sb = new StringBuilder();

            foreach (var name in Enum.GetNames(type))
            {
                var member = type.GetMember(name).First();
                var desc = member.GetCustomAttribute<DescriptionAttribute>()?.Description ?? name;
                var value = Convert.ToInt32(Enum.Parse(type, name));

                sb.AppendLine($"{value} = {name}（{desc}）\n");
            }

            enumDescriptions[apiParam.Name] = sb.ToString();
        }

        // 2. 再把说明写入 operation.Parameters
        foreach (var parameter in operation.Parameters)
        {
            if (enumDescriptions.TryGetValue(parameter.Name, out var desc))
            {
                var sb = new StringBuilder();
                sb.AppendLine(parameter.Description + "\n");
                sb.AppendLine(desc);
                parameter.Description = sb.ToString();
            }
        }
    }

}
