using System;
using EasyNetQ;
using Nano.Eventing.Interfaces;
using Serilog;

namespace Nano.Eventing.Providers.EasyNetQ
{
    /// <summary>
    /// EasyNetQ Provider.
    /// </summary>
    public class EasyNetQProvider : IEventingProvider
    {
        /// <summary>
        /// Logger.
        /// </summary>
        protected virtual ILogger Logger { get; }

        /// <summary>
        /// Options.
        /// </summary>
        protected virtual EventingOptions Options { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/>.</param>
        /// <param name="options">The <see cref="EventingOptions"/>.</param>
        public EasyNetQProvider(ILogger logger, EventingOptions options)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            this.Logger = logger;
            this.Options = options;
        }

        /// <inheritdoc />
        public virtual IEventing Configure()
        {
            var host = this.Options.Host;
            var port = this.Options.Port;
            var vhost = this.Options.VHost;
            var username = this.Options.AuthenticationCredential.Username;
            var password = this.Options.AuthenticationCredential.Password;
            var connectionString = $"amqp://{username}:{password}@{host}:{port}{vhost}";

            var bus = RabbitHutch.CreateBus(connectionString, y =>
            {
                y.Register(z => this.Logger);
                y.Register<IEasyNetQLogger, EasyNetQLogger>();
            });

            return new EasyNetQEventing(bus);
        }
    }
}