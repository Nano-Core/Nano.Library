using System.ComponentModel.DataAnnotations;
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
        /// Property One.
        /// </summary>
        [Required]
        public virtual string PropertyOne { get; set; }

        /// <summary>
        /// Property Two.
        /// </summary>
        public virtual string PropertyTwo { get; set; }

        /// <summary>
        /// Nested Example Entity.
        /// </summary>
        public virtual ExampleEntity NestedExampleEntity { get; set; }
    }
}