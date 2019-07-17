using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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
        public virtual object BytesToMessage(Type type, byte[] bytes)
        {
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(bytes), type, serializerSettings);
        }
        
        /// <inheritdoc />
        public virtual byte[] MessageToBytes(Type messageType, object message)
        {
            using (var memoryStream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();

                formatter
                    .Serialize(memoryStream, message);

                return memoryStream.ToArray();
            }
        }
    }
}