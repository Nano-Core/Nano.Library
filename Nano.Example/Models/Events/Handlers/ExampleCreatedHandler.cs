using System;
using Nano.Eventing.Interfaces;
using Nano.Services.Interfaces;
using Serilog;

namespace Nano.Example.Models.Events.Handlers
{
    /// <summary>
    /// Example Created Handler.
    /// </summary>
    public class ExampleCreatedHandler : IEventingHandler<ExampleCreatedEvent>
    {
        /// <summary>
        /// Logger.
        /// </summary>
        protected virtual ILogger Logger { get; }

        /// <summary>
        /// Service.
        /// </summary>
        protected virtual IService Service { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/>.</param>
        /// <param name="service">The <see cref="IService"/>.</param>
        public ExampleCreatedHandler(ILogger logger, IService service)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            if (service == null)
                throw new ArgumentNullException(nameof(service));

            this.Logger = logger;
            this.Service = service;
        }

        /// <inheritdoc />
        public void Callback(ExampleCreatedEvent @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            this.Logger.Information("Callback Invoked.");
        }
    }
}