using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;
using Nano.App.Api.Controllers;
using Nano.Data.Abstractions.Models.Abstractions;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nano.App.Api.Mvc.Documentation.Filters.Operation;

internal class ReplaceEntityResponseTypeFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        ArgumentNullException.ThrowIfNull(operation);
        ArgumentNullException.ThrowIfNull(context);

        var baseType = GetGenericBaseType(context.MethodInfo.DeclaringType!, typeof(BaseEntityViewController<,>));
        
        if (baseType == null)
        {
            return;
        }

        var entityType = baseType
            .GetGenericArguments()[0];

        var producesAttributes = context.MethodInfo
            .GetCustomAttributes(true)
            .OfType<ProducesResponseTypeAttribute>();

        foreach (var attr in producesAttributes)
        {
            if (!operation.Responses!.TryGetValue(attr.StatusCode.ToString(), out var response))
            {
                continue;
            }

            foreach (var content in response.Content!.Values)
            {
                if (content.Schema == null)
                {
                    continue;
                }

                if (attr.Type == typeof(IEntity))
                {
                    content.Schema = context.SchemaGenerator
                        .GenerateSchema(entityType, context.SchemaRepository);
                }

                else if (IsEnumerableOfIEntity(attr.Type))
                {
                    var listType = typeof(IEnumerable<>)
                        .MakeGenericType(entityType);

                    content.Schema = context.SchemaGenerator
                        .GenerateSchema(listType, context.SchemaRepository);
                }
            }
        }
    }


    private static bool IsEnumerableOfIEntity(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        if (!type.IsGenericType)
        {
            return false;
        }

        return type.GetGenericTypeDefinition() == typeof(IEnumerable<>) && type.GetGenericArguments()[0] == typeof(IEntity);
    }
    private static Type? GetGenericBaseType(Type type, Type genericBase)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(genericBase);

        while (type != typeof(object))
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == genericBase)
            {
                return type;
            }

            type = type.BaseType!;
        }

        return null;
    }
}