using Nano.Eventing.Attributes;
using Nano.Models;

namespace NanoCore.Example.Models
{
    /// <summary>
    /// Example Entity.
    /// </summary>
    [Publish]
    [Subscribe]
    public class ExampleEntity : DefaultEntity
    {
        /// <summary>
        /// Required.
        /// Property One.
        /// </summary>
        public virtual string PropertyOne { get; set; }

        /// <summary>
        /// Property Two.
        /// </summary>
        public virtual string PropertyTwo { get; set; }
    }
}