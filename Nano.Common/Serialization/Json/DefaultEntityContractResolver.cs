using System;
using System.Collections;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Nano.Common.Serialization.Json;

/// <summary>
/// A <see cref="DefaultContractResolver"/> that customizes JSON serialization for entities,
/// including support for serializing only non-empty enumerable properties.
/// </summary>
public class DefaultEntityContractResolver : DefaultContractResolver
{
    /// <summary>
    /// Creates a <see cref="JsonProperty"/> for the given <paramref name="member"/> and configures serialization rules, including non-empty enumerable handling.
    /// </summary>
    /// <param name="member">The <see cref="MemberInfo"/> representing the property or field.</param>
    /// <param name="memberSerialization">The <see cref="MemberSerialization"/> mode.</param>
    /// <returns>The configured <see cref="JsonProperty"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="member"/> is null.</exception>
    /// <exception cref="NullReferenceException">Thrown if the property type could not be determined.</exception>
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        ArgumentNullException.ThrowIfNull(member);

        var property = base.CreateProperty(member, memberSerialization);
        var propertyType = property.PropertyType;

        if (propertyType == null)
        {
            throw new NullReferenceException(nameof(propertyType));
        }

        this.SerializeOnlyNonEmptyEnumerables(member, propertyType, ref property);

        return property;
    }

    /// <summary>
    /// Configures a <see cref="JsonProperty"/> to serialize only if its enumerable value is non-empty.
    /// </summary>
    /// <param name="member">The <see cref="MemberInfo"/> representing the property or field.</param>
    /// <param name="propertyType">The <see cref="Type"/> of the property.</param>
    /// <param name="property">The <see cref="JsonProperty"/> to configure.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="member"/>, <paramref name="propertyType"/>, or <paramref name="property"/> is null.</exception>
    protected void SerializeOnlyNonEmptyEnumerables(MemberInfo member, Type propertyType, ref JsonProperty property)
    {
        ArgumentNullException.ThrowIfNull(member);
        ArgumentNullException.ThrowIfNull(propertyType);
        ArgumentNullException.ThrowIfNull(property);

        if (propertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(propertyType))
        {
            property.ShouldSerialize = instance =>
            {
                var enumerable = member.MemberType switch
                {
                    MemberTypes.Field => instance
                        .GetType()
                        .GetField(member.Name)?
                        .GetValue(instance) as IEnumerable,
                    MemberTypes.Property => instance
                        .GetType()
                        .GetProperty(member.Name)?
                        .GetValue(instance, null) as IEnumerable,
                    _ => null
                };

                // ReSharper disable NotDisposedResource
                return enumerable == null || enumerable.GetEnumerator().MoveNext();
                // ReSharper restore NotDisposedResource
            };
        }
    }
}