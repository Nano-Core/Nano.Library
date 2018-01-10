using System;

namespace Nano.Services.Eventing
{
    /// <summary>
    /// Entity Event.
    /// </summary>
    public class EntityEvent
    {
        /// <summary>
        /// Data.
        /// </summary>
        public virtual object Data { get; set; }

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public EntityEvent()
            : this(new object())
        {
            
        }   

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data">The <see cref="object"/> data.</param>
        public EntityEvent(object data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            this.Data = data;
        }
    }
}