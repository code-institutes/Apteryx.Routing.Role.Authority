using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace Apteryx.Routing.Role.Authority.Filters;

public class EnumDescriptionSchemaFilter : ISchemaFilter
{
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        var type = context.Type;

        if (!type.IsEnum)
            return;

        var sb = new StringBuilder();
        sb.AppendLine(schema.Description);
        sb.AppendLine("枚举说明：\n");

        foreach (var name in Enum.GetNames(type))
        {
            var member = type.GetMember(name).First();
            var desc = member.GetCustomAttribute<DescriptionAttribute>()?.Description ?? name;
            var value = Convert.ToInt32(Enum.Parse(type, name));

            sb.AppendLine($"{value} = {name}（{desc}）\n");
        }

        schema.Description = sb.ToString();
    }
}


