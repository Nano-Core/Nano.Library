using System.Linq;
using System.Reflection;
using Microsoft.OpenApi.Models;
using Nano.Models.Attributes;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nano.Web.Hosting.Documentation.Filters;

/// <summary>
/// Swagger Response Only Operation Filter.
/// </summary>
public class SwaggerResponseOnlyOperationFilter : IOperationFilter
{
    /// <inheritdoc />
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Responses == null)
        {
            return;
        }

        foreach (var response in operation.Responses)
        {
            if (response.Value.Content == null)
            {
                continue;
            }

            foreach (var content in response.Value.Content)
            {
                if (content.Value.Schema?.Reference == null)
                {
                    continue;
                }

                var schemaId = content.Value.Schema.Reference.Id;
                if (context.SchemaRepository.Schemas.TryGetValue(schemaId, out var schema))
                {
                    var responseOnlyProperties = context.MethodInfo.ReturnType
                        .GetProperties()
                        .Where(x => x.GetCustomAttribute<SwaggerResponseOnlyAttribute>() != null)
                        .Select(x => char.ToLowerInvariant(x.Name[0]) + x.Name[1..]);

                    foreach (var propertyName in responseOnlyProperties)
                    {
                        if (!schema.Properties.ContainsKey(propertyName))
                        {
                            var propertyInfo = context.MethodInfo.ReturnType
                                .GetProperty(propertyName);

                            if (propertyInfo == null)
                            {
                                continue;
                            }

                            var propSchema = context.SchemaGenerator
                                .GenerateSchema(propertyInfo.PropertyType, context.SchemaRepository);
                            
                            schema.Properties
                                .Add(propertyName, propSchema);
                        }
                    }
                }
            }
        }
    }
}