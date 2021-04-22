using System.Linq;
using System.Reflection;
using Microsoft.OpenApi.Models;
using Nano.Models.Attributes;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nano.Web.Hosting.Documentation.Filters
{
    /// <summary>
    /// Swagger Exclude Filter.
    /// </summary>
    public class SwaggerExcludeFilter : ISchemaFilter
    {
        /// <summary>
        /// Apply.
        /// </summary>
        /// <param name="schema">The <see cref="OpenApiSchema"/>.</param>
        /// <param name="context">The <see cref="SchemaFilterContext"/>.</param>
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema?.Properties == null || context.Type == null)
                return;

            var excludedProperties = context.Type
                .GetProperties()
                .Where(x => x
                    .GetCustomAttribute<SwaggerExcludeAttribute>() != null);

            foreach (var excludedProperty in excludedProperties)
            {
                if (schema.Properties.ContainsKey(excludedProperty.Name))
                {
                    schema.Properties
                        .Remove(excludedProperty.Name);
                }
            }
        }
    }
}