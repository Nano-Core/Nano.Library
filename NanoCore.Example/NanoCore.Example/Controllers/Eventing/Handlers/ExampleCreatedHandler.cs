using System;
using Microsoft.Extensions.Logging;
using Nano.Eventing.Interfaces;
using Nano.Services.Interfaces;
using NanoCore.Example.Models.Events;

namespace NanoCore.Example.Controllers.Eventing.Handlers
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

            this.Logger.LogInformation("Callback Invoked.");
        }
    }
}