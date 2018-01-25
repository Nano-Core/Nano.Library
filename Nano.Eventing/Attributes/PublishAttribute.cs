using System;

namespace Nano.Eventing.Attributes
{
    /// <summary>
    /// Publish Attribute.
    /// Types with this annotation, defines that an event will be published for the entity when it changes. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PublishAttribute : Attribute
    {
        /// <summary>
        /// Key.
        /// </summary>
        public virtual string Key { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="key">The routing key used when publishing.</param>
        public PublishAttribute(string key = null)
        {
            this.Key = key;
        }
    }
}