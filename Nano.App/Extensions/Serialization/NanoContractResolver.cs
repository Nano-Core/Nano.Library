using System.Collections;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Nano.App.Extensions.Serialization
{
    /// <inheritdoc />
    public class NanoContractResolver : DefaultContractResolver
    {
        /// <inheritdoc />
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            var propertyType = property.PropertyType;

            if (propertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(propertyType))
            {
                property.ShouldSerialize = instance =>
                {
                    IEnumerable enumerable = null;

                    switch (member.MemberType)
                    {
                        case MemberTypes.Field:
                            enumerable = instance
                                .GetType()
                                .GetField(member.Name)
                                .GetValue(instance) as IEnumerable;
                            break;

                        case MemberTypes.Property:
                            enumerable = instance
                                .GetType()
                                .GetProperty(member.Name)
                                .GetValue(instance, null) as IEnumerable;
                            break;
                    }

                    if (enumerable != null)
                        return enumerable.GetEnumerator().MoveNext();

                    return true;
                };
            }

            return property;
        }
    }
}