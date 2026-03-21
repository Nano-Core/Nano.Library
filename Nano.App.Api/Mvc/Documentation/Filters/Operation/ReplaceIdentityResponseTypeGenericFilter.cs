using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;
using Nano.App.Api.Controllers;
using Nano.Data.Abstractions.Models.Identity;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nano.App.Api.Mvc.Documentation.Filters.Operation;

internal class ReplaceIdentityResponseTypeGenericFilter : IOperationFilter
{
    private static readonly Type[] targetTypes =
    [
        typeof(IdentityUserEx<>),
        typeof(IdentityUserRole<>),
        typeof(IdentityUserClaim<>),
        typeof(IdentityUserLogin<>),
        typeof(IdentityUserToken<>),
        typeof(IdentityUserRefreshToken<>),
        typeof(IdentityUserChangeData<>),
        typeof(IdentityRole<>),
        typeof(IdentityRoleClaim<>),
        typeof(IdentityApiKey<>),
        typeof(IdentityApiKeyCreated<>)
    ];

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        ArgumentNullException.ThrowIfNull(operation);
        ArgumentNullException.ThrowIfNull(context);

        var baseType = GetGenericBaseType(context.MethodInfo.DeclaringType!, typeof(BaseIdentityController<,,>));

        if (baseType == null)
        {
            return;
        }

        var identityType = baseType
            .GetGenericArguments()[1];

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
                if (content.Schema == null || attr.Type == null)
                {
                    continue;
                }

                Type? fixedType = null;

                if (IsGenericEnumerable(attr.Type, out var innerType))
                {
                    var replacedInner = ReplaceGenericString(innerType!, identityType);

                    if (replacedInner != null)
                    {
                        fixedType = typeof(IEnumerable<>).MakeGenericType(replacedInner);
                    }
                }
                else
                {
                    fixedType = ReplaceGenericString(attr.Type, identityType);
                }

                if (fixedType != null)
                {
                    content.Schema = context.SchemaGenerator
                        .GenerateSchema(fixedType, context.SchemaRepository);
                }
            }
        }
    }


    private static bool IsGenericEnumerable(Type type, out Type? innerType)
    {
        ArgumentNullException.ThrowIfNull(type);

        innerType = null;

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            innerType = type
                .GetGenericArguments()[0];

            return true;
        }

        return false;
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
    private static bool ReplaceGenericString(Type type, Type identityType, out Type? replaced)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(identityType);

        replaced = null;

        if (!type.IsGenericType)
        {
            return false;
        }

        var genericDef = type
            .GetGenericTypeDefinition();

        if (!targetTypes.Select(t => t.GetGenericTypeDefinition()).Contains(genericDef))
        {
            return false;
        }

        var genericArgs = type
            .GetGenericArguments();

        for (var i = 0; i < genericArgs.Length; i++)
        {
            genericArgs[i] = identityType;
        }

        replaced = genericDef
            .MakeGenericType(genericArgs);

        return true;
    }
    private static Type? ReplaceGenericString(Type type, Type identityType)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(identityType);

        if (ReplaceGenericString(type, identityType, out var replaced))
        {
            return replaced;
        }

        return null;
    }
}