using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nano.Eventing.Interfaces;
using Nano.Models.Extensions;

namespace Nano.Eventing.Providers.EasyNetQ
{
    /// <inheritdoc />
    public class EasyNetQEventing : IEventing
    {
        /// <summary>
        /// Bus.
        /// </summary>
        protected virtual IBus Bus { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="bus">The <see cref="IBus"/>.</param>
        public EasyNetQEventing(IBus bus)
        {
            this.Bus = bus ?? throw new ArgumentNullException(nameof(bus));
        }

        /// <inheritdoc />
        public virtual async Task PublishAsync<TMessage>(TMessage body, string routing = "")
            where TMessage : class
        {
            if (body == null)
                throw new ArgumentNullException(nameof(body));

            var name = typeof(TMessage).GetFriendlyName();
            var message = new Message<TMessage>(body);

            var exchange = await this.Bus.Advanced
                .ExchangeDeclareAsync(name, ExchangeType.Fanout);

            await this.Bus.Advanced
                .PublishAsync(exchange, routing, true, message);
        }

        /// <inheritdoc />
        public virtual async Task SubscribeAsync<TMessage>(string routing = "", CancellationToken cancellationToken = default)
            where TMessage : class
        {
            var name = typeof(TMessage).GetFriendlyName();
            var route = string.IsNullOrEmpty(routing) ? string.Empty : $".{routing}";
            var appName = Assembly.GetEntryAssembly()?.GetName().Name;
            var queueName = $"{appName}:{name}{route}";

            var queue = await this.Bus.Advanced
                .QueueDeclareAsync($"{queueName}", cancellationToken);

            var exchange = await this.Bus.Advanced
                .ExchangeDeclareAsync(name, ExchangeType.Fanout, cancellationToken: cancellationToken);

            await this.Bus.Advanced
                .BindAsync(exchange, queue, routing, cancellationToken);

            var serviceCollection = this.Bus.Advanced.Container
                .Resolve<IServiceCollection>();

            this.Bus.Advanced
                .Consume<TMessage>(queue, (message, info) =>
                {
                    try
                    {
                        if (info.RoutingKey != routing)
                            return;

                        var eventType = message.MessageType;
                        var genericType = typeof(IEventingHandler<>)
                            .MakeGenericType(eventType);

                        var eventHandler = serviceCollection
                            .BuildServiceProvider()
                            .GetRequiredService(genericType);

                        var method = eventHandler
                            .GetType()
                            .GetMethod(nameof(IEventingHandler<object>.CallbackAsync));

                        if (method == null)
                            throw new NullReferenceException(nameof(method));

                        var callbackTask = (Task)method
                            .Invoke(eventHandler, new object[] { message.Body, info.Redelivered });

                        callbackTask?
                            .Wait(cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        var logger = serviceCollection
                            .BuildServiceProvider()
                            .GetRequiredService<ILogger>();

                        logger
                            .LogError(ex, ex.Message);

                        throw;
                    }
                });
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