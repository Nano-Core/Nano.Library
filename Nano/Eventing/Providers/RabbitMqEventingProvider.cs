using System;
using System.Text;
using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.Logging;
using Nano.App;
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
        protected ILogger<IEventingProvider> Logger { get; }

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
        public virtual void Fanout<TBody>(TBody body, bool persist = true)
            where TBody : class
        {
            if (body == null)
                throw new ArgumentNullException(nameof(body));

            try
            {
                var name = typeof(TBody).FullName;
                var exchange = this.Bus.Advanced.ExchangeDeclare(name, ExchangeType.Fanout);
                var message = new Message<TBody>(body)
                {
                    Properties =
                    {
                        AppId = BaseApplication.Name,
                        DeliveryMode = persist ? MessageDeliveryMode.Persistent : MessageDeliveryMode.NonPersistent,
                        CorrelationId = Guid.NewGuid().ToString("N")
                    }
                };

                this.Bus.Advanced
                    .Publish(exchange, string.Empty, true, message);

                this.Logger.LogInformation($"Executed Eventing, Message published{Environment.NewLine}Message: {JsonConvert.SerializeObject(message)}");
            }
            catch (Exception ex)
            {
                this.Logger.LogWarning(ex, ex.Message);
                this.Logger.LogInformation($"Executed Eventing, Message published failed{Environment.NewLine}Entity: {body}");

                throw;
            }
        }

        /// <inheritdoc />
        public virtual void Consume<TBody>(Action<byte[]> callback)
            where TBody : class
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            try
            {
                var name = typeof(TBody).FullName;
                var queue = this.Bus.Advanced.QueueDeclare(name);

                this.Bus.Advanced
                    .Consume(queue, (bytes, properties, info) =>
                    {
                        try
                        {
                            var message = Encoding.UTF8.GetString(bytes);

                            callback(bytes);
                            this.Logger.LogInformation($"Executed Eventing, Message consume successfully{Environment.NewLine}Message: {message}");
                        }
                        catch (Exception ex)
                        {
                            this.Logger.LogWarning(ex, ex.Message);
                            this.Logger.LogWarning(ex, $"Executed Eventing, Message consume failed{Environment.NewLine}Message: {ex.Message}");
                        }
                    });
            }
            catch (Exception ex)
            {
                this.Logger.LogWarning(ex, ex.Message);
                this.Logger.LogWarning(ex, $"Executed Eventing, Message consume failed{Environment.NewLine}Message: {ex.Message}");

                throw;
            }
        }

        /// <inheritdoc />
        public virtual void Consume<TBody>(Action<TBody> callback, string routingKey = "", bool isTemporary = false)
            where TBody : class
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            try
            {
                var queue = isTemporary
                    ? this.Bus.Advanced.QueueDeclare($"{BaseApplication.Name}:{Guid.NewGuid():D}", false, false, false, false, null, 300000)
                    : this.Bus.Advanced.QueueDeclare($"{typeof(TBody).FullName}:{BaseApplication.Name}");

                this.Bus.Advanced
                    .Consume<TBody>(queue, (message, info) =>
                    {
                        try
                        {
                            callback(message.Body);
                            this.Logger.LogInformation($"Executed Eventing, Message consume successfully{Environment.NewLine}Message: {message}");
                        }
                        catch (Exception ex)
                        {
                            this.Logger.LogWarning(ex, ex.Message);
                            this.Logger.LogWarning(ex, $"Executed Eventing, Message consume failed{Environment.NewLine}Message: {ex.Message}");
                        }
                    });

                var name = typeof(TBody).FullName;
                var exchange = this.Bus.Advanced
                    .ExchangeDeclare(name, ExchangeType.Fanout);

                this.Bus.Advanced
                    .Bind(exchange, queue, routingKey);

            }
            catch (Exception ex)
            {
                this.Logger.LogWarning(ex, ex.Message);
                this.Logger.LogWarning(ex, $"Executed Eventing, Message consume failed{Environment.NewLine}Message: {ex.Message}");

                throw;
            }
        }

        /// <inheritdoc />
        public virtual void Respond<TRequest, TResponse>(Func<TRequest, TResponse> callback)
            where TRequest : class
            where TResponse : class
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            this.Bus
                .Respond(callback);
        }

        /// <inheritdoc />
        public virtual TResponse Request<TRequest, TResponse>(TRequest request)
            where TRequest : class
            where TResponse : class
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return this.Bus
                .Request<TRequest, TResponse>(request);
        }

        /// <inheritdoc />
        public virtual void Receive<TBody>(Action<TBody> callback)
            where TBody : class
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            this.Bus
                .Receive(typeof(TBody).FullName, (TBody message) =>
                {
                    try
                    {
                        callback(message);
                        this.Logger.LogInformation($"Message consume successfully{Environment.NewLine}Message: {message}");
                    }
                    catch (Exception exception)
                    {
                        this.Logger.LogWarning(exception, $"Message consume failed{Environment.NewLine}Message: {exception.Message}");
                    }
                });
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Bus?.Dispose();
            }
        }
    }
}