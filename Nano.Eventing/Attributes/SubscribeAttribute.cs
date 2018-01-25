using System;

namespace Nano.Eventing.Attributes
{
    /// <summary>
    /// Subscribe Attribute.
    /// Types with this annotation, subscribes to events of the declaring type. 
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
        /// <param name="key">The routing key used when subscribing.</param>
        public SubscribeAttribute(string key = null)
        {
            this.Key = key;
        }
    }
}