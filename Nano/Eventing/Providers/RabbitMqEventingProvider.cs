using System;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.Logging;
using Nano.Api.Entities.Interfaces;
using Nano.Eventing.Providers.Interfaces;
using Newtonsoft.Json;

namespace Nano.Eventing.Providers
{
    /// <summary>
    /// RabbitMQ Eventing.
    /// </summary>
    public class RabbitMqEventingProvider : IEventingProvider
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
        public RabbitMqEventingProvider(IBus bus, ILogger<IEventingProvider> logger)
        {
            if (bus == null)
                throw new ArgumentNullException(nameof(bus));

            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            this.Bus = bus;
            this.Logger = logger;
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
                var deliverymode = persist ? MessageDeliveryMode.Persistent : MessageDeliveryMode.NonPersistent;

                var message = new Message<TBody>(body)
                {
                    Properties =
                    {
                        AppId = "",
                        DeliveryMode = deliverymode,
                        CorrelationId = $"{Guid.NewGuid():N}"
                    }
                };

                await this.Bus.Advanced
                    .PublishAsync(exchange, string.Empty, true, message);

                this.Logger.LogInformation($"Executed Eventing, Message Publish Succeeded{Environment.NewLine}Message: {JsonConvert.SerializeObject(message)}");
            }
            catch (Exception ex)
            {
                this.Logger.LogInformation($"Executed Eventing, Message Publish Failed{Environment.NewLine}Exception: {ex.Message}");
                throw;
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
                this.Logger.LogInformation($"Executed Eventing, Message Request Failed{Environment.NewLine}Exception: {ex.Message}");
                throw;
            }
        }

        /// <inheritdoc />
        public virtual void Consume<TBody>(Action<byte[]> callback)
            where TBody : class
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            var name = typeof(TBody).FullName;
            var queue = this.Bus.Advanced.QueueDeclare(name);

            this.Bus.Advanced.Consume(queue, (bytes, properties, info) => callback(bytes));
        }

        /// <inheritdoc />
        public virtual void Consume<TBody>(Action<TBody> callback, bool isTemporary = false)
            where TBody : class
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            var name = typeof(TBody).FullName;
            var exchange = this.Bus.Advanced.ExchangeDeclare(name, ExchangeType.Fanout);
            var queue = !isTemporary
                ? this.Bus.Advanced.QueueDeclare($"{typeof(TBody).FullName}")
                : this.Bus.Advanced.QueueDeclare($"{Guid.NewGuid():N}", false, false, false, false, null, 300000);

            this.Bus.Advanced.Consume<TBody>(queue, (message, info) => callback(message.Body));
            this.Bus.Advanced.Bind(exchange, queue, "");
        }

        /// <inheritdoc />
        public virtual void Receive<TBody>(Action<TBody> callback)
            where TBody : class
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            var name = typeof(TBody).FullName;
            this.Bus.Receive(name, callback);
        }

        /// <inheritdoc />
        public virtual void Respond<TRequest, TResponse>(Func<TRequest, TResponse> callback)
            where TRequest : class, IRequest
            where TResponse : class, IResponse
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            this.Bus.Respond(callback);
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