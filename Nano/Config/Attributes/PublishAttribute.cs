using System;
using Microsoft.EntityFrameworkCore;

namespace Nano.Config.Attributes
{
    /// <summary>
    /// Eventing Attribute.
    /// Types annotated with this <see cref="Attribute"/> will publish a message changes to the instance is saved. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class PublishAttribute : Attribute
    {
        /// <summary>
        /// States.
        /// </summary>
        public virtual EntityState[] States { get; set; }

        /// <summary>
        /// Constructor.
        /// Initializing <see cref="States"/> with <see cref="EntityState.Added"/>, <see cref="EntityState.Modified"/> and <see cref="EntityState.Deleted"/>.
        /// </summary>
        public PublishAttribute()
            : this(EntityState.Added, EntityState.Modified, EntityState.Deleted)
        {
            
        }

        /// <summary>
        /// Constructor.
        /// Initializing <see cref="States"/> with the passed <paramref name="states"/>
        /// </summary>
        /// <param name="states">The <see cref="EntityState"/>'s.</param>
        public PublishAttribute(params EntityState[] states)
        {
            this.States = states;
        }
    }
}