using System;
using System.IO;
using System.Xml.Serialization;

namespace Nano.Hosting.Extensions
{
    /// <summary>
    /// Xml Convert.
    /// </summary>
    public static class XmlConvert
    {
        /// <summary>
        /// Serializes an instance of type <typeparamref name="T"/> to xml.
        /// </summary>
        /// <typeparam name="T">The type to serialize.</typeparam>
        /// <param name="object">The object instance to serialize.</param>
        /// <param name="defaultNamespace">The default namespace used in the xml.</param>
        /// <param name="namespaces">The namespaces.</param>
        /// <param name="knownTypes">The known types.</param>
        /// <returns>A <see cref="string"/> formatted as xml.</returns>
        public static string SerializeObject<T>(T @object, string defaultNamespace = null, XmlSerializerNamespaces namespaces = null, Type[] knownTypes = null)
        {
            var xmlSerializer = new XmlSerializer(typeof(T), null, knownTypes, null, defaultNamespace);

            using (var stringWriter = new StringWriter())
            {
                xmlSerializer.Serialize(stringWriter, @object, namespaces);
                return stringWriter.ToString();
            }
        }

        /// <summary>
        /// Deserializes an xml strong to an instance of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the xml into.</typeparam>
        /// <param name="xml">The xml to deserialize.</param>
        /// <param name="defaultNamespace">The default namespace used in the xml.</param>
        /// <param name="knownTypes">The known types.</param>
        /// <returns>The instance of type <typeparamref name="T"/>.</returns>
        public static T DeserializeObject<T>(string xml, string defaultNamespace = null, Type[] knownTypes = null)
        {
            var xmlSerializer = new XmlSerializer(typeof(T), null, knownTypes, null, defaultNamespace);

            using (var stringReader = new StringReader(xml))
            {
                return (T)xmlSerializer.Deserialize(stringReader);
            }
        }
    }
}