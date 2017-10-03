using Nano.App.Models;

namespace Nano.Example.Models
{
    /// <summary>
    /// Example Entity.
    /// </summary>
    public class ExampleEntity : DefaultEntity
    {
        /// <summary>
        /// Required.
        /// Property One.
        /// </summary>
        public virtual bool PropertyOne { get; set; }

        /// <summary>
        /// Required.
        /// Property Two.
        /// </summary>
        public virtual string PropertyTwo { get; set; }
    }
}