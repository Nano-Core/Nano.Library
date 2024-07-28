using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Nano.Models.Serialization.Json;

/// <inheritdoc />
public class DefaultEntityContractResolver : BaseEntityContractResolver
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

        return property;
    }
}