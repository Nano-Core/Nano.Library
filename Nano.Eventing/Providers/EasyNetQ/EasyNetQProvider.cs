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
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            this.Options = options;
        }

        /// <inheritdoc />
        public virtual IEventing Configure()
        {
            var host = this.Options.Host;
            var port = this.Options.Port;
            var vhost = this.Options.VHost;
            var username = this.Options.Username;
            var password = this.Options.Password;
            var connectionString = $"amqp://{username}:{password}@{host}:{port}{vhost}";

            var bus = RabbitHutch.CreateBus(connectionString, x => 
                x.Register<ISerializer, EasyNetQJsonSerializer>());

            return new EasyNetQEventing(bus);
        }
    }
}