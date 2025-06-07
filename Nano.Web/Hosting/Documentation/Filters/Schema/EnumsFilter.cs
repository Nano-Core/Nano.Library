using System;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nano.Web.Hosting.Documentation.Filters.Schema;

/// <summary>
/// Enums Filter.
/// </summary>
public class EnumsFilter : ISchemaFilter
{
    /// <inheritdoc />
    public void Apply(OpenApiSchema model, SchemaFilterContext context)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        if (context == null)
            throw new ArgumentNullException(nameof(context));

        if (!context.Type.IsEnum)
        {
            return;
        }

        model.Enum
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

            model.Enum
                .Add(new OpenApiString(label));
        }
    }
}