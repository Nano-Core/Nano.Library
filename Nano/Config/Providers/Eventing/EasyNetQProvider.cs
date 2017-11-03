using System;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.Logging;
using Nano.Api.Requests.Interfaces;
using Nano.Api.Responses.Interfaces;
using Nano.Config.Providers.Eventing.Interfaces;
using Newtonsoft.Json;

namespace Nano.Config.Providers.Eventing
{
    /// <summary>
    /// EasyNetQ Provider.
    /// </summary>
    public class EasyNetQProvider : IEventingProvider
    {
        /// <summary>
        /// Bus.
        /// </summary>
        protected virtual IBus Bus { get; }

        /// <summary>
        /// Logger.
        /// </summary>
        protected virtual ILogger<IEventingProvider> Logger { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="bus">The <see cref="IBus"/>.</param>
        public EasyNetQProvider(IBus bus, ILogger<IEventingProvider> logger)
        {
            if (bus == null)
                throw new ArgumentNullException(nameof(bus));

            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            this.Bus = bus;
            this.Logger = logger;
        }

        /// <inheritdoc />
        public virtual IBus Configure(EventingOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            var host = options.Host;
            var port = options.Port;
            var vhost = options.VHost;
            var username = options.AuthenticationCredential.Username;
            var password = options.AuthenticationCredential.Password;
            var connectionString = $"amqp://{username}:{password}@{host}:{port}{vhost}";

            return RabbitHutch.CreateBus(connectionString, y =>
            {
                y.Register<IEasyNetQLogger>(z => z.Resolve<EasyNetQLogger>());
            });
        }

        /// <inheritdoc />
        public virtual async Task PublishAsync<TBody>(TBody body, bool persist = true, CancellationToken cancellationToken = default)
            where TBody : class
        {
            if (body == null)
                throw new ArgumentNullException(nameof(body));

            try
            {
                var name = typeof(TBody).FullName;
                var exchange = this.Bus.Advanced.ExchangeDeclare(name, ExchangeType.Fanout);
                var message = new Message<TBody>(body);

                await this.Bus.Advanced
                    .PublishAsync(exchange, string.Empty, true, message);

                this.Logger.LogInformation($"Executed Eventing, Message Publish Succeeded{Environment.NewLine}Message: {JsonConvert.SerializeObject(message)}");
            }
            catch (Exception ex)
            {
                this.Logger.LogWarning($"Executed Eventing, Message Publish Failed{Environment.NewLine}Exception: {ex.Message}");
            }
        }

        /// <inheritdoc />
        public virtual async Task<TResponse> SendRequestAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
            where TRequest : class, IRequest
            where TResponse : class, IResponse
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            try
            {
                var response = await this.Bus
                    .RequestAsync<TRequest, TResponse>(request);

                this.Logger.LogInformation($"Executed Eventing, Message Request Succeeded{Environment.NewLine}Request: {JsonConvert.SerializeObject(request)}");

                return response;
            }
            catch (Exception ex)
            {
                this.Logger.LogWarning($"Executed Eventing, Message Request Failed{Environment.NewLine}Exception: {ex.Message}");
                throw;
            }
        }

        /// <inheritdoc />
        public virtual void Consume<TMessage>(Action<byte[]> callback)
            where TMessage : class
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            try
            {
                var name = typeof(TMessage).FullName;
                var queue = this.Bus.Advanced.QueueDeclare(name);

                this.Bus.Advanced
                    .Consume(queue, (bytes, properties, info) => callback(bytes));

                this.Logger.LogInformation("Executed Eventing, Message Consume Succeeded.");
            }
            catch (Exception ex)
            {
                this.Logger.LogWarning($"Executed Eventing, Message Consume Failed{Environment.NewLine}Exception: {ex.Message}");
            }
        }

        /// <inheritdoc />
        public virtual void Consume<TMessage>(Action<TMessage> callback, bool isTemporary = false)
            where TMessage : class
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            try
            {
                var name = typeof(TMessage).FullName;
                var exchange = this.Bus.Advanced.ExchangeDeclare(name, ExchangeType.Fanout);
                var queue = !isTemporary
                    ? this.Bus.Advanced.QueueDeclare($"{name}")
                    : this.Bus.Advanced.QueueDeclare($"{Guid.NewGuid():N}", false, false, false, false, null, 300000);

                this.Bus.Advanced.Consume<TMessage>(queue, (message, info) => callback(message.Body));
                this.Bus.Advanced.Bind(exchange, queue, string.Empty);

                this.Logger.LogInformation("Executed Eventing, Message Consume Succeeded.");
            }
            catch (Exception ex)
            {
                this.Logger.LogWarning($"Executed Eventing, Message Consume Failed{Environment.NewLine}Exception: {ex.Message}");
            }
        }

        /// <inheritdoc />
        public virtual void Receive<TMessage>(Action<TMessage> callback)
            where TMessage : class
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            try
            {
                var name = typeof(TMessage).FullName;

                this.Bus
                    .Receive(name, callback);

                this.Logger.LogInformation("Executed Eventing, Message Receive Succeeded.");
            }
            catch (Exception ex)
            {
                this.Logger.LogWarning($"Executed Eventing, Message Receive Failed{Environment.NewLine}Exception: {ex.Message}");
            }
        }

        /// <inheritdoc />
        public virtual void Respond<TRequest, TResponse>(Func<TRequest, TResponse> callback)
            where TRequest : class, IRequest
            where TResponse : class, IResponse
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            try
            {
                this.Bus
                    .Respond(callback);

                this.Logger.LogInformation("Executed Eventing, Message Respond Succeeded.");
            }
            catch (Exception ex)
            {
                this.Logger.LogWarning($"Executed Eventing, Message Respond Failed{Environment.NewLine}Exception: {ex.Message}");
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose.
        /// Only disposes if passed <paramref name="disposing"/> is true.
        /// </summary>
        /// <param name="disposing">The <see cref="bool"/> indicating if disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Bus?.Dispose();
            }
        }
    }
}