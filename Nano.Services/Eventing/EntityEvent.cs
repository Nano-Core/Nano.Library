using System;
using Microsoft.EntityFrameworkCore;

namespace Nano.Services.Eventing
{
    /// <summary>
    /// Entity Event.
    /// </summary>
    public class EntityEvent
    {
        /// <summary>
        /// Id.
        /// </summary>
        public virtual object Id { get; set; }

        /// <summary>
        /// Type.
        /// </summary>
        public virtual string Type { get; set; }

        /// <summary>
        /// State.
        /// </summary>
        public virtual EntityState State { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="state"></param>
        public EntityEvent(object id, string type, EntityState state)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            if (type == null)
                throw new ArgumentNullException(nameof(type));

            this.Id = id;
            this.Type = type;
            this.State = state;
        }
    }
}