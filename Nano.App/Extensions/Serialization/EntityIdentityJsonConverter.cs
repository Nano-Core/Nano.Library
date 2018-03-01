using System;
using Nano.Models.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Nano.App.Extensions.Serialization
{
    /// <inheritdoc />
    public class EntityIdentityJsonConverter<TIdentity> : JsonConverter
    {
        /// <inheritdoc />
        public override bool CanConvert(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return type == typeof(IEntityIdentity<TIdentity>);
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));

            // BUG: Issue 2: The EntityIdentity<> Json conversion doesn't set Id.
            var token = JToken.FromObject(value);
            var propertyToken = token.SelectToken("Id");

            if (propertyToken == null)
            {
                token.WriteTo(writer);
            }
            else
            {
                propertyToken.WriteTo(writer);
            }
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            if (objectType == null)
                throw new ArgumentNullException(nameof(objectType));

            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));

            var token = JToken.Load(reader);

            return token;
        }
    }
}