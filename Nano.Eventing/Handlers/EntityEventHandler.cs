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
        public virtual async void CallbackAsync(EntityEvent @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var type = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsTypeDef(typeof(IEntityIdentity<>)))
                .First(x => x.Name == @event.Type);

            var id = type.IsTypeDef(typeof(IEntityIdentity<Guid>))
                ? new Guid(@event.Id.ToString())
                : @event.Id;

            var entity = await this.Context
                .FindAsync(type, id);

            switch (@event.State)
            {
                case "Added":
                    if (entity != null)
                        return;

                    var property = type.GetProperty("Id");

                    if (property == null)
                        return;

                    entity = Activator.CreateInstance(type);
                    property.SetValue(entity, id);

                    await this.Context.AddAsync(entity);
                    await this.Context.SaveChangesAsync();

                    return;

                case "Deleted":
                    if (entity == null)
                        return;

                    this.Context.Remove(entity);
                    await this.Context.SaveChangesAsync();

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