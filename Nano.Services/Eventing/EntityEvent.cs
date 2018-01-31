using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Nano.Models.Interfaces;

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
        public virtual string RoutingKey { get; set; }

        /// <summary>
        /// State.
        /// </summary>
        public virtual EntityState State { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entity">The <see cref="object"/> entity.</param>
        public EntityEvent(EntityEntry<IEntityIdentity<Guid>> entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            this.Id = entity.Entity.Id;
            this.State = entity.State;
        }
    }
}