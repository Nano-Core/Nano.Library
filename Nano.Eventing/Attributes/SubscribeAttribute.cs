using System;

namespace Nano.Eventing.Attributes
{
    /// <summary>
    /// Eventing Attribute.
    /// Types annotated with this <see cref="Attribute"/> will publish a message changes to the instance is saved. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SubscribeAttribute : Attribute
    {
        /// <summary>
        /// Key.
        /// </summary>
        public virtual string Key { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="key">The routing key when subscribing.</param>
        public SubscribeAttribute(string key = null)
        {
            this.Key = key;
        }
    }
}