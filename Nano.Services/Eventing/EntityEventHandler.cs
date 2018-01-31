using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Nano.Eventing.Interfaces;
using Nano.Models;
using Nano.Services.Interfaces;

namespace Nano.Services.Eventing
{
    /// <summary>
    /// Entity Event Handler.
    /// </summary>
    public class EntityEventHandler : IEventingHandler<EntityEvent>
    {
        /// <summary>
        /// Service.
        /// </summary>
        protected virtual IService Service { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="service">The <see cref="IService"/>.</param>
        public EntityEventHandler(IService service)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            this.Service = service;
        }

        /// <inheritdoc />
        public async void CallbackAsync(EntityEvent @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            // BUG: Eventing Annotation: Entity Eventing Handler callback
            var type = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .FirstOrDefault(x => x.Name == @event.RoutingKey);

            var entity = await this.Service
                .GetAsync<DefaultEntity, Guid>((Guid)@event.Id);

            switch (@event.State)
            {
                case EntityState.Deleted:
                    await this.Service.DeleteAsync(entity);
                    break;

                case EntityState.Added:
                    if (entity == null)
                    {
                        await this.Service
                            .AddAsync(new DefaultEntity { Id = (Guid)@event.Id });
                    }

                    break;
            }
        }
    }
}