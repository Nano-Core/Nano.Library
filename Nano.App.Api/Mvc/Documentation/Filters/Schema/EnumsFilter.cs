using System;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nano.App.Api.Mvc.Documentation.Filters.Schema;

/// <summary>
/// Enums Filter.
/// </summary>
public class EnumsFilter : ISchemaFilter
{
    /// <inheritdoc />
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        if (!context.Type.IsEnum)
        {
            return;
        }

        if (schema.Enum == null)
        {
            return;
        }

        schema.Enum
            .Clear();

        var enumNames = Enum.GetNames(context.Type);

        foreach (var enumName in enumNames)
        {
            var memberInfo = context.Type
                .GetMember(enumName)
                .FirstOrDefault(m => m.DeclaringType == context.Type);

            var enumMemberAttribute = memberInfo == null
                ? null
                : memberInfo
                    .GetCustomAttributes(typeof(EnumMemberAttribute), false)
                    .OfType<EnumMemberAttribute>()
                    .FirstOrDefault();

            var label = enumMemberAttribute == null || string.IsNullOrWhiteSpace(enumMemberAttribute.Value)
                ? enumName
                : enumMemberAttribute.Value;

            schema.Enum
                .Add(label);
        }
    }
}