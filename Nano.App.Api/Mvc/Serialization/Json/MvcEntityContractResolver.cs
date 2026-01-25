using System;
using System.Reflection;
using Nano.Common.Extensions;
using Nano.Common.Serialization.Json;
using Nano.Data.Abstractions.Annotations;
using Nano.Data.Abstractions.Entities.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Nano.App.Api.Mvc.Serialization.Json;

internal sealed class MvcEntityContractResolver : DefaultEntityContractResolver
{
    /// <summary>
    /// Create a property.
    /// </summary>
    /// <param name="member">The <see cref="MemberInfo"/>.</param>
    /// <param name="memberSerialization">The <see cref="MemberSerialization"/>.</param>
    /// <returns>The <see cref="JsonProperty"/>.</returns>
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        ArgumentNullException.ThrowIfNull(member);

        var property = base.CreateProperty(member, memberSerialization);
        var propertyType = property.PropertyType;

        if (propertyType == null)
        {
            throw new NullReferenceException(nameof(propertyType));
        }

        SerializeOnlyIncludedProperties(member, propertyType, ref property);

        return property;
    }


    private static void SerializeOnlyIncludedProperties(MemberInfo member, Type propertyType, ref JsonProperty property)
    {
        if (member == null)
            throw new ArgumentNullException(nameof(member));

        ArgumentNullException.ThrowIfNull(propertyType);
        ArgumentNullException.ThrowIfNull(property);

        if (!propertyType.IsTypeOf(typeof(IEntity)) && (!propertyType.IsGenericType || !propertyType.GenericTypeArguments[0].IsTypeOf(typeof(IEntity))))
        {
            return;
        }

        var includeAnnotation = member
            .GetCustomAttribute<IncludeAttribute>();

        property.ShouldSerialize = _ => includeAnnotation != null;
    }
}