using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi;
using Nano.Data.Abstractions.Models;
using Nano.Data.Abstractions.Models.Abstractions;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nano.App.Api.Mvc.Documentation.Filters.Operation;

internal class RequestUserOperationFilter : IOperationFilter
{
    private const string SUFFIX = "Request";

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        ArgumentNullException.ThrowIfNull(operation);
        ArgumentNullException.ThrowIfNull(context);

        if (operation.RequestBody?.Content == null)
        {
            return;
        }

        var bodyType = context.ApiDescription.ParameterDescriptions
            .Where(x => x.Source == BindingSource.Body)
            .Select(x => x.Type)
            .FirstOrDefault();

        if (bodyType == null)
        {
            return;
        }

        var type = GetItemType(bodyType);

        if (!ReachesUser(type, []))
        {
            return;
        }

        var requestId = CreateRequestSchema(type, context);

        if (requestId == null)
        {
            return;
        }

        foreach (var media in operation.RequestBody.Content.Values)
        {
            if (media.Schema != null)
            {
                media.Schema = Replace(media.Schema, requestId);
            }
        }
    }


    private static string? CreateRequestSchema(Type type, OperationFilterContext context)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(context);

        var response = context.SchemaGenerator
            .GenerateSchema(type, context.SchemaRepository);

        var responseId = (response as OpenApiSchemaReference)?.Reference.Id;

        if (responseId == null || context.SchemaRepository.Schemas[responseId] is not OpenApiSchema source)
        {
            return null;
        }

        var requestId = responseId + SUFFIX;

        if (context.SchemaRepository.Schemas.ContainsKey(requestId))
        {
            return requestId;
        }

        var copy = (OpenApiSchema)source.CreateShallowCopy();

        var properties = source.Properties?
            .ToDictionary(x => x.Key, x => x.Value);

        copy.Properties = properties;
        copy.Required = source.Required?
            .ToHashSet();

        context.SchemaRepository
            .AddDefinition(requestId, copy);

        if (properties == null)
        {
            return requestId;
        }

        var isUser = IsUser(type);

        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var name = properties.Keys
                .FirstOrDefault(x => string.Equals(x, property.Name, StringComparison.OrdinalIgnoreCase));

            if (name == null)
            {
                continue;
            }

            if (isUser && property.Name == nameof(BaseEntityUser.IdentityUser))
            {
                properties
                    .Remove(name);

                copy.Required?
                    .Remove(name);

                continue;
            }

            var propertyType = GetItemType(property.PropertyType);

            if (!ReachesUser(propertyType, []))
            {
                continue;
            }

            var propertyRequestId = CreateRequestSchema(propertyType, context);

            if (propertyRequestId != null)
            {
                properties[name] = Replace(properties[name], propertyRequestId);
            }
        }

        return requestId;
    }
    private static bool ReachesUser(Type type, HashSet<Type> visited)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(visited);

        if (!visited.Add(type))
        {
            return false;
        }

        if (IsUser(type))
        {
            return true;
        }

        var isLeaf = type.IsValueType || type == typeof(string) || type == typeof(object) || type.Namespace?.StartsWith("System", StringComparison.Ordinal) == true || type.Namespace?.StartsWith("Microsoft", StringComparison.Ordinal) == true;

        if (isLeaf)
        {
            return false;
        }

        return type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Any(x => ReachesUser(GetItemType(x.PropertyType), visited));
    }
    private static bool IsUser(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return type
            .GetInterfaces()
            .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEntityUser<>));
    }
    private static Type GetItemType(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        if (type == typeof(string))
        {
            return type;
        }

        if (type.IsArray)
        {
            return type.GetElementType() ?? type;
        }

        var itemType = type
            .GetInterfaces()
            .Append(type)
            .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            .Select(x => x.GetGenericArguments()[0])
            .FirstOrDefault();

        return itemType ?? type;
    }
    private static IOpenApiSchema Replace(IOpenApiSchema schema, string requestId)
    {
        ArgumentNullException.ThrowIfNull(schema);
        ArgumentNullException.ThrowIfNull(requestId);

        if (schema is OpenApiSchemaReference)
        {
            return new OpenApiSchemaReference(requestId);
        }

        if (schema is not OpenApiSchema concrete)
        {
            return schema;
        }

        var copy = (OpenApiSchema)concrete.CreateShallowCopy();

        copy.Items = concrete.Items == null ? null : Replace(concrete.Items, requestId);
        copy.AllOf = concrete.AllOf?
            .Select(x => Replace(x, requestId))
            .ToList();

        return copy;
    }
}
