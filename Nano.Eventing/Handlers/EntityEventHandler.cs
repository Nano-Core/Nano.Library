using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Nano.Eventing.Interfaces;
using Nano.Models;
using Nano.Models.Extensions;
using Nano.Models.Interfaces;

namespace Nano.Eventing.Handlers
{
    /// <summary>
    /// Entity Event Handler.
    /// </summary>
    public class EntityEventHandler : IEventingHandler<EntityEvent>
    {
        /// <summary>
        /// Context.
        /// </summary>
        protected virtual DbContext Context { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">The <see cref="DbContext"/>.</param>
        public EntityEventHandler(DbContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            this.Context = context;
        }

        /// <inheritdoc />
        public void CallbackAsync(EntityEvent @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var type = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsTypeDef(typeof(IEntityIdentity<>)))
                .FirstOrDefault(x => x.Name == @event.Type);

            var entity = this.Context.Find(type, @event.Id);

            switch (@event.State)
            {
                case "Added":
                    if (entity == null)
                    {
                        entity = Activator.CreateInstance(type);
                        this.Context.Add(entity);
                    }
                    return;

                case "Deleted":
                    this.Context.Remove(entity);
                    return;

                case "Detached":
                case "Unchanged":
                case "Modified":
                    return;

                default:
                    return;
            }
        }
    }
}