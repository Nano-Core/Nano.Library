using Nano.App.Models;
using Nano.Eventing.Attributes;

namespace Nano.Example.Models
{
    /// <summary>
    /// Example Entity.
    /// </summary>
    [Eventing]
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