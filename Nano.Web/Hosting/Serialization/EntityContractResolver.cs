using System;
using System.Collections;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Nano.Web.Hosting.Serialization
{
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

            if (propertyType == typeof(ILazyLoader))
            {
                property.Ignored = true;
            }
            else if (propertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(propertyType))
            {
                property.ShouldSerialize = instance =>
                {
                    var enumerable = member.MemberType switch
                    {
                        MemberTypes.Field => instance
                            .GetType()
                            .GetField(member.Name)?
                            .GetValue(instance) as IEnumerable,

                        MemberTypes.Property => instance.GetType().GetProperty(member.Name)?.GetValue(instance, null) as
                            IEnumerable,

                        _ => null
                    };

                    return enumerable == null || enumerable.GetEnumerator().MoveNext();
                };
            }

            return property;
        }
    }
}