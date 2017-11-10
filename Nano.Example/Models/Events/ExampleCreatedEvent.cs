using System;

namespace Nano.Example.Models.Events
{
    /// <summary>
    /// Example Created Event.
    /// </summary>
    public class ExampleCreatedEvent
    {
        /// <summary>
        /// Id.
        /// </summary>
        public virtual Guid Id { get; set; }

        /// <summary>
        /// Property One.
        /// </summary>
        public virtual string PropertyOne { get; set; }

        /// <summary>
        /// Property Two.
        /// </summary>
        public virtual string PropertyTwo { get; set; }
    }
}