using System;
using Nano.Eventing.Interfaces;
using Nano.Services.Interfaces;

namespace Nano.Services.Eventing.Handlers
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
        public void Callback(EntityEvent @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            // TODO: EVENTING
            //switch (@event.State)
            //{
            //    case "added":
            //        var type = Type.GetType(@event.Name);
            //        var instance = Activator.CreateInstance(type);



            //        //this.Service.Add(entity);
            //        break;

            //    case "updated":
            //        break;

            //    case "delete":
            //        //var a = this.Service.Get<T>(@event.Id).Result;
            //        //this.Service.Delete(a);
            //        break;
            //}
        }
    }
}