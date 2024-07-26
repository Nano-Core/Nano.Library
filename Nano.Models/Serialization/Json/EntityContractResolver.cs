using System;
using System.Collections;
using System.Reflection;
using Nano.Models.Attributes;
using Nano.Models.Extensions;
using Nano.Models.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Nano.Models.Serialization.Json;

/// <inheritdoc />
public class EntityContractResolver : DefaultContractResolver
{
    /// <summary>
    /// Create a property.
    /// Empty collections is not serialzied.
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
    private void SerializeOnlyNonEmptyEnumerables(MemberInfo member, Type propertyType, ref JsonProperty property)
    {
        if (member == null) 
            throw new ArgumentNullException(nameof(member));
        
        if (propertyType == null) 
            throw new ArgumentNullException(nameof(propertyType));
        
        if (property == null) 
            throw new ArgumentNullException(nameof(property));

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