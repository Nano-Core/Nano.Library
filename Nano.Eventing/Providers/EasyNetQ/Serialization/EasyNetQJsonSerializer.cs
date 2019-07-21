using System;
using System.Text;
using EasyNetQ;
using Newtonsoft.Json;

namespace Nano.Eventing.Providers.EasyNetQ.Serialization
{
    /// <summary>
    /// EasyNetQ Json Serializer.
    /// </summary>
    public class EasyNetQJsonSerializer : ISerializer
    {
        private readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.None
        };

        /// <inheritdoc />
        public virtual byte[] MessageToBytes<T>(T message) 
            where T : class
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message, serializerSettings));
        }

        /// <inheritdoc />
        public virtual T BytesToMessage<T>(byte[] bytes)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(bytes), serializerSettings);
        }

        /// <inheritdoc />
        public virtual object BytesToMessage(Type type, byte[] bytes)
        {
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(bytes), type, serializerSettings);
        }
    }
}