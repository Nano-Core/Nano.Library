using Nano.Config.Attributes;
using Nano.Models;

namespace Nano.Example.Models
{
    /// <summary>
    /// Example Entity.
    /// </summary>
    [Publish]
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