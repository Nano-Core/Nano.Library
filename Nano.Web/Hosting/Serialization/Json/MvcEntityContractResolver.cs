using System;
using System.Reflection;
using Nano.Models.Attributes;
using Nano.Models.Extensions;
using Nano.Models.Interfaces;
using Nano.Models.Serialization.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Nano.Web.Hosting.Serialization.Json;

/// <inheritdoc />
public class MvcEntityContractResolver : BaseEntityContractResolver
{
    /// <summary>
    /// Create a property.
    /// </summary>
    /// <param name="member">The <see cref="MemberInfo"/>.</param>
    /// <param name="memberSerialization">The <see cref="MemberSerialization"/>.</param>
    /// <returns>The <see cref="JsonProperty"/>.</returns>
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        if (member == null)
            throw new ArgumentNullException(nameof(member));

        var property = base.CreateProperty(member, memberSerialization);
        var propertyType = property.PropertyType;

        if (propertyType == null)
        {
            throw new NullReferenceException(nameof(propertyType));
        }

        this.SerializeOnlyNonEmptyEnumerables(member, propertyType, ref property);
        this.SerializeOnlyIncludedProperties(member, propertyType, ref property);

        return property;
    }

    private void SerializeOnlyIncludedProperties(MemberInfo member, Type propertyType, ref JsonProperty property)
    {
        if (member == null)
            throw new ArgumentNullException(nameof(member));

        if (propertyType == null)
            throw new ArgumentNullException(nameof(propertyType));

        if (property == null)
            throw new ArgumentNullException(nameof(property));

        if (propertyType.IsTypeOf(typeof(IEntity)) || (propertyType.IsGenericType && propertyType.GenericTypeArguments[0].IsTypeOf(typeof(IEntity))))
        {
            var includeAnnotation = member
                .GetCustomAttribute<IncludeAttribute>();

            if (includeAnnotation == null)
            {
                property.ShouldSerialize = _ => false;
            }
        }
    }
}