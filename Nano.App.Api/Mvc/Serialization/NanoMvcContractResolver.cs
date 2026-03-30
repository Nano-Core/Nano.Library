using Microsoft.Extensions.Options;
using Nano.Common.Serialization;
using Nano.Data.Abstractions.Annotations;
using Nano.Data.Abstractions.Config;
using Nano.Data.Abstractions.Models.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Nano.App.Api.Mvc.Serialization;

internal sealed class NanoMvcContractResolver(IOptions<DataOptions>? dataOptions = null) : NanoDefaultContractResolver
{
    private readonly IOptions<DataOptions>? dataOptions = dataOptions;

    private static readonly AsyncLocal<Stack<object>?> currentStack = new();
    private static Stack<object> CurrentStack => currentStack.Value ??= new Stack<object>();

    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        ArgumentNullException.ThrowIfNull(member);

        var property = base.CreateProperty(member, memberSerialization);

        var propertyType = property.PropertyType;

        if (!IsEntityNavigation(propertyType!))
        {
            return property;
        }

        var include = member
            .GetCustomAttribute<IncludeAttribute>();

        property.ShouldSerialize = _ =>
        {
            if (include == null)
            {
                return false;
            }

            var depth = CurrentStack.Count;
            var maxDepth = dataOptions?.Value.Repository.QueryIncludeDepth ?? int.MaxValue;

            return depth <= maxDepth;
        };

        return property;
    }

    protected override JsonObjectContract CreateObjectContract(Type objectType)
    {
        ArgumentNullException.ThrowIfNull(objectType);

        var contract = base.CreateObjectContract(objectType);

        if (!typeof(IEntity).IsAssignableFrom(objectType))
        {
            return contract;
        }

        contract.OnSerializingCallbacks
            .Add((x, _) =>
            {
                var stack = CurrentStack;

                if (!stack.Contains(x))
                {
                    stack
                        .Push(x);
                }
            });

        contract.OnSerializedCallbacks
            .Add((x, _) =>
            {
                var stack = CurrentStack;

                if (stack.Count > 0 && ReferenceEquals(stack.Peek(), x))
                {
                    stack
                        .Pop();
                }
            });

        return contract;
    }


    private static bool IsEntityNavigation(Type propertyType)
    {
        if (typeof(IEntity).IsAssignableFrom(propertyType))
        {
            return true;
        }

        if (typeof(IEnumerable).IsAssignableFrom(propertyType) && propertyType.IsGenericType)
        {
            var genericArg = propertyType.GenericTypeArguments[0];

            if (typeof(IEntity).IsAssignableFrom(genericArg))
            {
                return true;
            }
        }

        return false;
    }
}