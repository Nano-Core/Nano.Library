using System;
using EasyNetQ;
using Nano.Eventing.Interfaces;
using Nano.Eventing.Providers.EasyNetQ.Serialization;

namespace Nano.Eventing.Providers.EasyNetQ
{
    /// <summary>
    /// EasyNetQ Provider.
    /// </summary>
    public class EasyNetQProvider : IEventingProvider
    {
        /// <summary>
        /// Options.
        /// </summary>
        protected virtual EventingOptions Options { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="options">The <see cref="EventingOptions"/>.</param>
        public EasyNetQProvider(EventingOptions options)
        {
            this.Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <inheritdoc />
        public virtual IEventing Configure()
        {
            var bus = RabbitHutch.CreateBus(this.Options.ConnectionString, x => 
                x.Register<ISerializer, EasyNetQJsonSerializer>());

            return new EasyNetQEventing(bus);
        }
    }
}