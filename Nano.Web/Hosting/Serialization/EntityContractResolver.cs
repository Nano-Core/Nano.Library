using System.Collections;
using System.Reflection;
using Nano.Models.Extensions;
using Nano.Models.Interfaces;
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
        /// Properties of types implementing <see cref="IEntityIdentity{TIdentity}"/> is not serialized. 
        /// </summary>
        /// <param name="member">The <see cref="MemberInfo"/>.</param>
        /// <param name="memberSerialization">The <see cref="MemberSerialization"/>.</param>
        /// <returns>The <see cref="JsonProperty"/>.</returns>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            var propertyType = property.PropertyType;

            if (propertyType.IsTypeDef(typeof(IEntityIdentity<>)))
            {
                if (property.DeclaringType.IsTypeDef(typeof(IEntity)))
                {
                    property.Ignored = true;
                }
            }
            else if (propertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(propertyType))
            {
                if (property.DeclaringType.IsTypeDef(typeof(IEntity)))
                {
                    property.Ignored = true;
                }
                else
                {
                    property.ShouldSerialize = instance =>
                    {
                        IEnumerable enumerable = null;

                        switch (member.MemberType)
                        {
                            case MemberTypes.Field:
                                enumerable = instance
                                    .GetType()
                                    .GetField(member.Name)?
                                    .GetValue(instance) as IEnumerable;
                                break;

                            case MemberTypes.Property:
                                enumerable = instance
                                    .GetType()
                                    .GetProperty(member.Name)?
                                    .GetValue(instance, null) as IEnumerable;
                                break;
                        }

                        if (enumerable != null)
                            return enumerable.GetEnumerator().MoveNext();

                        return true;
                    };
                }
            }

            return property;
        }
    }
}