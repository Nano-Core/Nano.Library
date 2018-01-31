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
        /// Routing Key.
        /// </summary>
        public virtual string RoutingKey { get; set; } // TODO: Entity Event Routing Key.

        /// <summary>
        /// State.
        /// </summary>
        public virtual EntityState State { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="routingKey"></param>
        /// <param name="state"></param>
        public EntityEvent(object id, string routingKey, EntityState state)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            if (routingKey == null)
                throw new ArgumentNullException(nameof(routingKey));

            this.Id = id;
            this.RoutingKey = routingKey;
            this.State = state;
        }
    }
}