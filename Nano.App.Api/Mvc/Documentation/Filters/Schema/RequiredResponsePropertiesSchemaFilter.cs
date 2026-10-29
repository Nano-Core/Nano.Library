using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nano.App.Api.Mvc.Documentation.Filters.Schema;

/// <summary>
/// Swagger schema filter that marks the properties of a model that are value types, and not nullable, as required, because they are always sent in a response.
/// A model that is only sent to the API in a request body is left as it is: a value type there has a default, and is required only when it is marked so.
/// A model that is both, such as an entity, is marked, as the response sends every value.
/// </summary>
public sealed class RequiredResponsePropertiesSchemaFilter(IApiDescriptionGroupCollectionProvider apiDescriptionProvider)
    : ISchemaFilter
{
    private readonly Lazy<(HashSet<Type> Requests, HashSet<Type> Responses)> usage = new(() => Read(apiDescriptionProvider ?? throw new ArgumentNullException(nameof(apiDescriptionProvider))));

    /// <summary>
    /// Applies the filter to mark the value type properties of a response model as required.
    /// </summary>
    /// <param name="schema">The <see cref="IOpenApiSchema"/> to modify.</param>
    /// <param name="context">The <see cref="SchemaFilterContext"/> providing type information.</param>
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        ArgumentNullException.ThrowIfNull(schema);
        ArgumentNullException.ThrowIfNull(context);

        if (schema is not OpenApiSchema openApiSchema || openApiSchema.Properties == null || context.Type == null)
        {
            return;
        }

        if (this.usage.Value.Requests.Contains(context.Type) && !this.usage.Value.Responses.Contains(context.Type))
        {
            return;
        }

        var required = context.Type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(x => x.PropertyType.IsValueType && Nullable.GetUnderlyingType(x.PropertyType) == null)
            .Select(x => openApiSchema.Properties.Keys.FirstOrDefault(y => string.Equals(y, x.Name, StringComparison.OrdinalIgnoreCase)))
            .Where(x => x != null);

        foreach (var name in required)
        {
            openApiSchema.Required ??= new HashSet<string>();
            openApiSchema.Required
                .Add(name!);
        }
    }


    private static (HashSet<Type> Requests, HashSet<Type> Responses) Read(IApiDescriptionGroupCollectionProvider provider)
    {
        var descriptions = provider.ApiDescriptionGroups.Items
            .SelectMany(x => x.Items)
            .ToArray();

        var requests = new HashSet<Type>();
        var responses = new HashSet<Type>();

        foreach (var description in descriptions)
        {
            var bodies = description.ParameterDescriptions
                .Where(x => x.Source == BindingSource.Body)
                .Select(x => x.Type);

            foreach (var type in bodies)
            {
                Collect(type, requests);
            }

            var returned = description.SupportedResponseTypes
                .Select(x => x.Type)
                .Where(x => x != null);

            foreach (var type in returned)
            {
                Collect(type!, responses);
            }
        }

        return (requests, responses);
    }
    private static void Collect(Type type, HashSet<Type> found)
    {
        if (!found.Add(type))
        {
            return;
        }

        foreach (var argument in type.GetGenericArguments())
        {
            Collect(argument, found);
        }

        if (type.IsArray && type.GetElementType() is { } element)
        {
            Collect(element, found);
        }

        if (type.IsPrimitive || type.IsEnum || type == typeof(string) || type.Namespace?.StartsWith("System", StringComparison.Ordinal) == true || typeof(IEnumerable).IsAssignableFrom(type))
        {
            return;
        }

        var properties = type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            Collect(property.PropertyType, found);
        }
    }
}
