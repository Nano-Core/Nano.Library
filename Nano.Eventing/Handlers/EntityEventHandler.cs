using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nano.Eventing.Interfaces;
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
        /// Logger.
        /// </summary>
        protected virtual ILogger Logger { get; }

        /// <summary>
        /// Context.
        /// </summary>
        protected virtual DbContext Context { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/>.</param>
        /// <param name="context">The <see cref="DbContext"/>.</param>
        public EntityEventHandler(ILogger logger, DbContext context)
        {
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <inheritdoc />
        public virtual async Task CallbackAsync(EntityEvent @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            try
            {
                var type = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(x => x.GetTypes())
                    .Where(x => x.IsTypeOf(typeof(IEntityIdentity<>)))
                    .First(x => x.Name == @event.Type);

                Guid.TryParse(@event.Id.ToString(), out var guid);

                var id = type.IsTypeOf(typeof(IEntityIdentity<Guid>))
                    ? guid
                    : @event.Id;

                var entity = await this.Context
                    .FindAsync(type, id);

                switch (@event.State)
                {
                    case "Added":
                        if (entity != null)
                        {
                            this.Logger.LogInformation($"Nano: Subscribed to entity: {type.Name}, with Id: {id} already exists. Add entity ignored.");
                            return;
                        }

                        var property = type.GetProperty("Id");

                        if (property == null)
                        {
                            this.Logger.LogWarning($"Nano: Subscribed to entity: {type.Name}, does not cotnain an 'Id' property. Add entity ignored.");
                            return;
                        }

                        entity = Activator.CreateInstance(type);

                        if (entity == null)
                            throw new NullReferenceException(nameof(entity));

                        property.SetValue(entity, id);

                        await this.Context
                            .AddAsync(entity);

                        await this.Context
                            .SaveChangesAsync();

                        this.Logger.LogInformation($"Nano: Subscribed to entity: {type.Name}, with Id: {id} has been added.");

                        return;

                    case "Deleted":
                        var isSoftDeleted = entity is IEntityDeletableSoft deleted && deleted.IsDeleted > 0L;

                        if (entity == null || isSoftDeleted)
                        {
                            this.Logger.LogInformation($"Nano: Subscribed to entity: {type.Name}, with Id: {id} doesn't exists. Remove entity ignored.");
                            return;
                        }

                        this.Context
                            .Remove(entity);
                        
                        await this.Context
                            .SaveChangesAsync();

                        this.Logger.LogInformation($"Nano: Subscribed to entity: {type.Name}, with Id: {id} has been removed.");

                        return;

                    case "Detached":
                    case "Unchanged":
                    case "Modified":
                        return;

                    default:
                        return;
                }
            }
            catch (Exception ex)
            {
                this.Logger.Log(LogLevel.Error, ex, ex.Message);
                this.Logger.LogError($"Nano: Entity with id '{@event.Id}' of type: {@event.Type} with state: {@event.State} throw an exception.");
            }
        }
    }
}