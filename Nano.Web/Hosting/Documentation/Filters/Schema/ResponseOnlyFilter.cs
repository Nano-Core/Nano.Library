using System.Linq;
using System.Reflection;
using Microsoft.OpenApi.Models;
using Nano.Models.Attributes;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nano.Web.Hosting.Documentation.Filters.Schema;

/// <summary>
/// Response Only Filter.
/// </summary>
public class ResponseOnlyFilter : ISchemaFilter
{
    /// <inheritdoc />
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema?.Properties == null || context?.Type == null)
        {
            return;
        }

        var responseOnlyProperties = context.Type
            .GetProperties()
            .Where(x => x.GetCustomAttribute<SwaggerResponseOnlyAttribute>() != null)
            .Select(x => char.ToLowerInvariant(x.Name[0]) + x.Name[1..])
            .Where(x => schema.Properties.ContainsKey(x));

        foreach (var propName in responseOnlyProperties)
        {
            schema.Properties
                .Remove(propName);
        }
    }
}