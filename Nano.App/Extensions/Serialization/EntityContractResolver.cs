using System;
using System.Linq;
using System.Reflection;
using Nano.Models.Extensions;
using Nano.Models.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Nano.App.Extensions.Serialization
{
    /// <inheritdoc />
    public class EntityContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// Create a property.
        /// If property type implements <see cref="IEntityIdentity{TIdentity}"/> the property is not serialized. 
        /// Instead, the property name is suffixed with 'Id' and just the <see cref="IEntityIdentity{TIdentity}.Id"/> is serialized.
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
                var genericType = this.GetGenericTypeArgument(propertyType);
                var identityType = genericType?.GenericTypeArguments.FirstOrDefault();
                var jsonConverter = identityType == typeof(Guid) ? new EntityIdentityJsonConverter<Guid>() : (JsonConverter)new EntityIdentityJsonConverter<dynamic>();

                property.PropertyName += "Id";
                property.PropertyType = identityType;
                property.Converter = jsonConverter;
            }

            return property;
        }

        private Type GetGenericTypeArgument(Type propertyType)
        {
            if (propertyType == null)
                throw new ArgumentNullException(nameof(propertyType));

            return propertyType
                .GetParentTypes()
                .FirstOrDefault(x =>
                    x.IsGenericType &&
                    x.GetGenericTypeDefinition() == typeof(IEntityIdentity<>));
        }
    }
}